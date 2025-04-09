using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebBanPhuKienDienThoai.Extensions;
using WebBanPhuKienDienThoai.Models;
using WebBanPhuKienDienThoai.Services;

namespace WebBanPhuKienDienThoai.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PayPalService _payPalService;

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepository productRepository, PayPalService payPalService)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
            _payPalService = payPalService;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            var discount = HttpContext.Session.GetObjectFromJson<decimal>("DiscountAmount", 0m);
            var viewModel = new ShoppingCartViewModel
            {
                Cart = cart,
                TotalPrice = cart.Items.Sum(item => item.Price * item.Quantity),
                Discount = discount
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index");
            }

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                OrderDetails = cart.Items.Select(i => new OrderDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    Product = _context.Products.Include(p => p.Images).FirstOrDefault(p => p.Id == i.ProductId)
                }).ToList(),
                TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity)
            };

            var discountCode = HttpContext.Session.GetString("DiscountCode");
            if (!string.IsNullOrEmpty(discountCode))
            {
                var discount = await _context.DiscountCodes
                    .FirstOrDefaultAsync(d => d.Code == discountCode && d.ExpiryDate >= DateTime.Now && d.UsageCount < d.UsageLimit);

                if (discount != null)
                {
                    decimal discountAmount = 0;
                    if (discount.DiscountPercent.HasValue)
                    {
                        discountAmount = order.TotalPrice * (decimal)(discount.DiscountPercent.Value / 100);
                    }
                    else if (discount.DiscountAmount.HasValue)
                    {
                        discountAmount = discount.DiscountAmount.Value;
                    }
                    order.TotalPrice -= discountAmount;
                    HttpContext.Session.SetObjectAsJson("DiscountAmount", discountAmount); // Dùng Session
                    TempData["DiscountSuccess"] = $"Đã áp dụng mã giảm giá: {(discount.DiscountPercent.HasValue ? $"{discount.DiscountPercent}%" : $"{discount.DiscountAmount:N0} VNĐ")}";
                }
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order, string paymentMethod)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index");
            }

            foreach (var item in cart.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                {
                    return Json(new { success = false, message = $"Sản phẩm {item.Name} không đủ hàng." });
                }
            }

            var user = await _userManager.GetUserAsync(User);
            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);

            var discount = HttpContext.Session.GetObjectFromJson<decimal>("DiscountAmount", 0m); // Lấy từ Session
            if (discount > 0)
            {
                order.TotalPrice -= discount;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                product.Stock -= item.Quantity;
                _context.Products.Update(product);
            }
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("DiscountCode");
            HttpContext.Session.Remove("DiscountAmount"); // Xóa discount sau khi hoàn tất

            if (paymentMethod == "PayPal")
            {
                var returnUrl = Url.Action("PaymentSuccess", "Paypal", null, Request.Scheme);
                var cancelUrl = Url.Action("PaymentCancel", "Paypal", null, Request.Scheme);
                return RedirectToAction("CreatePayment", "Paypal", new { orderId = order.Id, totalPrice = order.TotalPrice, returnUrl, cancelUrl });
            }

            return View("OrderCompleted", order.Id);
        }

        [AllowAnonymous]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            int currentQuantity = existingItem?.Quantity ?? 0;
            int totalQuantity = currentQuantity + quantity;

            if (product == null || product.Stock < totalQuantity)
            {
                return Json(new { success = false, message = "Số lượng sản phẩm không đủ trong kho.", itemCount = cart.GetTotalQuantity() });
            }

            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity,
                Stock = product.Stock
            };
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            decimal itemTotal = product.Price * totalQuantity;
            return Json(new
            {
                success = true,
                message = "Đã thêm vào giỏ hàng!",
                itemCount = cart.GetTotalQuantity(),
                price = product.Price,
                itemTotal = itemTotal
            });
        }

        public IActionResult GetCartSummary()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            var totalPrice = cart.Items.Sum(item => item.Price * item.Quantity);
            var discount = HttpContext.Session.GetObjectFromJson<decimal>("DiscountAmount", 0m); // Lấy từ Session
            return Json(new { totalPrice, discount });
        }

        [AllowAnonymous]
        public async Task<IActionResult> CompareProducts(string productIds)
        {
            if (string.IsNullOrEmpty(productIds))
            {
                return View(new List<Product>());
            }
            var ids = productIds.Split(',').Select(int.Parse).ToArray();
            var products = await _productRepository.GetProductsByIdsAsync(ids);
            return View(products);
        }

        public async Task<IActionResult> OrderHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> ApplyDiscount(string discountCode)
        {
            if (string.IsNullOrEmpty(discountCode))
            {
                TempData["DiscountError"] = "Vui lòng nhập mã giảm giá.";
            }
            else
            {
                var discount = await _context.DiscountCodes
                    .FirstOrDefaultAsync(d => d.Code == discountCode && d.ExpiryDate >= DateTime.Now && d.UsageCount < d.UsageLimit);

                if (discount == null)
                {
                    TempData["DiscountError"] = "Mã giảm giá không hợp lệ, đã hết hạn hoặc đã dùng hết số lần cho phép.";
                }
                else
                {
                    var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
                    if (cart == null || !cart.Items.Any())
                    {
                        TempData["DiscountError"] = "Giỏ hàng trống.";
                    }
                    else
                    {
                        decimal totalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
                        decimal discountAmount = 0;
                        if (discount.DiscountPercent.HasValue)
                        {
                            discountAmount = totalPrice * (decimal)(discount.DiscountPercent.Value / 100);
                        }
                        else if (discount.DiscountAmount.HasValue)
                        {
                            discountAmount = discount.DiscountAmount.Value;
                        }
                        HttpContext.Session.SetObjectAsJson("DiscountAmount", discountAmount);
                        HttpContext.Session.SetString("DiscountCode", discount.Code);
                        TempData["DiscountSuccess"] = $"Đã áp dụng mã giảm giá: {(discount.DiscountPercent.HasValue ? $"{discount.DiscountPercent}%" : $"{discount.DiscountAmount:N0} VNĐ")}";
                    }
                }
            }

            // Tạo ShoppingCartViewModel để trả về
            var updatedCart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            var updatedDiscount = HttpContext.Session.GetObjectFromJson<decimal>("DiscountAmount", 0m);
            var viewModel = new ShoppingCartViewModel
            {
                Cart = updatedCart,
                TotalPrice = updatedCart.Items.Sum(item => item.Price * item.Quantity),
                Discount = updatedDiscount
            };
            return View("Index", viewModel);
        }
        private async Task<Product> GetProductFromDatabase(int productId)
        {
            return await _productRepository.GetByIdAsync(productId);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart != null)
            {
                cart.RemoveItem(productId);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            // Tạo ShoppingCartViewModel để trả về
            var discount = HttpContext.Session.GetObjectFromJson<decimal>("DiscountAmount", 0m);
            var viewModel = new ShoppingCartViewModel
            {
                Cart = cart,
                TotalPrice = cart.Items.Sum(item => item.Price * item.Quantity),
                Discount = discount
            };
            return View("Index", viewModel);
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart != null)
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == productId);
                if (quantity > product.Stock)
                {
                    TempData["Error"] = $"Sản phẩm chỉ còn {product.Stock} trong kho.";
                }
                else if (quantity < 1)
                {
                    TempData["Error"] = "Số lượng không thể nhỏ hơn 1.";
                }
                else
                {
                    var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                    if (item != null)
                    {
                        item.Quantity = quantity;
                        HttpContext.Session.SetObjectAsJson("Cart", cart);
                    }
                }
            }

            // Tạo ShoppingCartViewModel để trả về
            var discount = HttpContext.Session.GetObjectFromJson<decimal>("DiscountAmount", 0m);
            var viewModel = new ShoppingCartViewModel
            {
                Cart = cart,
                TotalPrice = cart.Items.Sum(item => item.Price * item.Quantity),
                Discount = discount
            };
            return View("Index", viewModel);
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("DiscountAmount"); // Xóa discount khi xóa giỏ hàng
            return Json(new { success = true, itemCount = 0 });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddRating(int productId, int stars, string review)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var rating = new Rating
            {
                ProductId = productId,
                UserId = userId,
                Stars = stars,
                Review = review,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Ratings.AddAsync(rating);
            await _context.SaveChangesAsync();

            return RedirectToAction("OrderHistory");
        }
    }
}