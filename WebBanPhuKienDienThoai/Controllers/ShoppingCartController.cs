using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhuKienDienThoai.Extensions;
using WebBanPhuKienDienThoai.Models;
using WebBanPhuKienDienThoai.Repositories;

namespace WebBanPhuKienDienThoai.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDiscountRepository _discountRepository;

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepository productRepository, IDiscountRepository discountRepository)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
            _discountRepository = discountRepository;
        }


        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index");
            }

            // Create a new order with initial details
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

            // Process discount code if available
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

                    // Store discount information
                    order.DiscountAmount = discountAmount;
                    order.TotalPrice -= discountAmount;
                    order.DiscountCodeId = discount.Id;

                    // Save the discount amount to TempData for display
                    TempData["DiscountAmount"] = discountAmount;
                    TempData["DiscountSuccess"] = $"Đã áp dụng mã giảm giá: {(discount.DiscountPercent.HasValue ? $"{discount.DiscountPercent}%" : $"{discount.DiscountAmount:N0} VNĐ")}";
                }
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;

            // Calculate total price before discount
            decimal totalBeforeDiscount = cart.Items.Sum(i => i.Price * i.Quantity);
            order.TotalPrice = totalBeforeDiscount;

            // Set order details
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();

            // Process discount if available
            var discountCode = HttpContext.Session.GetString("DiscountCode");
            if (!string.IsNullOrEmpty(discountCode))
            {
                var discount = await _context.DiscountCodes
                    .FirstOrDefaultAsync(d => d.Code == discountCode && d.ExpiryDate >= DateTime.Now && d.UsageCount < d.UsageLimit);

                if (discount != null)
                {
                    // Calculate discount amount
                    decimal discountAmount = 0;
                    if (discount.DiscountPercent.HasValue)
                    {
                        discountAmount = totalBeforeDiscount * (decimal)(discount.DiscountPercent.Value / 100);
                    }
                    else if (discount.DiscountAmount.HasValue)
                    {
                        discountAmount = discount.DiscountAmount.Value;
                    }

                    // Set discount properties on order
                    order.DiscountCodeId = discount.Id;
                    order.DiscountAmount = discountAmount;
                    order.TotalPrice -= discountAmount;

                    // Update discount usage count
                    discount.UsageCount++;
                    _context.Update(discount);
                }
            }

            // Add order to database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Update product stock
            foreach (var item in cart.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    _context.Products.Update(product);
                }
            }
            await _context.SaveChangesAsync();

            // Clear session
            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("DiscountCode");
            HttpContext.Session.Remove("DiscountAmount");

            return View("OrderCompleted", order.Id);
        }


        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null || product.Stock < quantity)
            {
                return Json(new { success = false, message = "Số lượng sản phẩm không đủ trong kho." });
            }

            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity
            };
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return Json(new { success = true, message = "Đã thêm vào giỏ hàng!" });
        }


        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            // Load thông tin Product cho các item trong giỏ hàng
            if (cart.Items != null && cart.Items.Any())
            {
                var productIds = cart.Items.Select(i => i.ProductId).ToList();
                var products = _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToList();

                foreach (var item in cart.Items)
                {
                    item.Product = products.FirstOrDefault(p => p.Id == item.ProductId);
                }
            }

            return View(cart);
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
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng trống" });
            }

            var discount = await _discountRepository.GetByCodeAsync(discountCode);
            if (discount == null || !discount.IsActive)
            {
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ" });
            }

            if (discount.ExpiryDate < DateTime.Now)
            {
                return Json(new { success = false, message = "Mã giảm giá đã hết hạn" });
            }

            if (discount.UsageCount >= discount.UsageLimit)
            {
                return Json(new { success = false, message = "Mã giảm giá đã hết lượt sử dụng" });
            }

            decimal totalPrice = cart.GetTotalPrice();
            decimal discountValue = 0;

            if (discount.DiscountAmount.HasValue)
            {
                discountValue = Math.Min(discount.DiscountAmount.Value, totalPrice);
            }
            else if (discount.DiscountPercent.HasValue)
            {
                discountValue = totalPrice * (decimal)(discount.DiscountPercent.Value / 100);
            }

            HttpContext.Session.SetString("DiscountCode", discount.Code);
            HttpContext.Session.SetString("DiscountAmount", discountValue.ToString());

            return Json(new
            {
                success = true,
                message = $"Áp dụng mã {discount.Code} thành công",
                discountAmount = discountValue.ToString("#,##0"),
                finalPrice = (totalPrice - discountValue).ToString("#,##0")
            });
        }

        [HttpPost]
        public IActionResult RemoveDiscount()
        {
            HttpContext.Session.Remove("DiscountCode");
            HttpContext.Session.Remove("DiscountAmount");
            return Json(new { success = true });
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
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart != null)
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == productId);
                if (product == null || quantity > product.Stock)
                {
                    return Json(new { success = false, message = "Số lượng sản phẩm không đủ trong kho." });
                }
                cart.UpdateItem(productId, quantity);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return Json(new { success = true, itemCount = cart?.GetTotalQuantity() ?? 0 });
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return Json(new { success = true, itemCount = 0 });
        }
    }
}