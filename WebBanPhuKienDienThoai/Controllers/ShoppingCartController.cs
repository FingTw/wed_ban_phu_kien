using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhuKienDienThoai.Extensions;
using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
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
                    TempData["DiscountAmount"] = discountAmount;
                    TempData["DiscountSuccess"] = $"Đã áp dụng mã giảm giá: {(discount.DiscountPercent.HasValue ? $"{discount.DiscountPercent}%" : $"{discount.DiscountAmount:N0} VNĐ")}";
                }
            }

            return View(order);
        }

        //[HttpPost]
        //public async Task<IActionResult> Checkout(Order order)
        //{
        //    var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
        //    if (cart == null || !cart.Items.Any())
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    var user = await _userManager.GetUserAsync(User);
        //    order.UserId = user.Id;
        //    order.OrderDate = DateTime.UtcNow;
        //    order.OrderDetails = cart.Items.Select(i => new OrderDetail
        //    {
        //        ProductId = i.ProductId,
        //        Quantity = i.Quantity,
        //        Price = i.Price
        //    }).ToList();
        //    order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);

        //    var discountCode = HttpContext.Session.GetString("DiscountCode");
        //    if (!string.IsNullOrEmpty(discountCode))
        //    {
        //        var discount = await _context.DiscountCodes
        //            .FirstOrDefaultAsync(d => d.Code == discountCode && d.ExpiryDate >= DateTime.Now && d.UsageCount < d.UsageLimit);
        //        if (discount != null)
        //        {
        //            decimal discountAmount = 0;
        //            if (discount.DiscountPercent.HasValue)
        //            {
        //                discountAmount = order.TotalPrice * (decimal)(discount.DiscountPercent.Value / 100);
        //            }
        //            else if (discount.DiscountAmount.HasValue)
        //            {
        //                discountAmount = discount.DiscountAmount.Value;
        //            }
        //            order.TotalPrice -= discountAmount;
        //            discount.UsageCount++;
        //            _context.Update(discount);
        //        }
        //    }

        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();
        //    HttpContext.Session.Remove("Cart");
        //    HttpContext.Session.Remove("DiscountCode");
        //    return View("OrderCompleted", order.Id);
        //}
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
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);

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
                    discount.UsageCount++;
                    _context.Update(discount);
                }
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Cập nhật lại số lượng tồn kho của sản phẩm
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

            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("DiscountCode");
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
            if (string.IsNullOrEmpty(discountCode))
            {
                return Json(new { success = false, message = "Vui lòng nhập mã giảm giá." });
            }

            var discount = await _context.DiscountCodes
                .FirstOrDefaultAsync(d => d.Code == discountCode && d.ExpiryDate >= DateTime.Now && d.UsageCount < d.UsageLimit);

            if (discount == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ, đã hết hạn hoặc đã dùng hết số lần cho phép." });
            }

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng trống." });
            }

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

            TempData["DiscountAmount"] = discountAmount;
            HttpContext.Session.SetString("DiscountCode", discount.Code);
            TempData["DiscountSuccess"] = $"Đã áp dụng mã giảm giá: {(discount.DiscountPercent.HasValue ? $"{discount.DiscountPercent}%" : $"{discount.DiscountAmount:N0} VNĐ")}";
            return Json(new { success = true });
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