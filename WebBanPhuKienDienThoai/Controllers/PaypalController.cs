using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebBanPhuKienDienThoai.Services;
using WebBanPhuKienDienThoai.Extensions;
using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Controllers
{
    public class PayPalController : Controller
    {
        private readonly PayPalService _payPalService;
        private readonly ApplicationDbContext _context;
         private readonly IProductRepository _productRepository;
         private readonly UserManager<ApplicationUser> _userManager;

        public PayPalController(PayPalService payPalService,  UserManager<ApplicationUser> userManager, ApplicationDbContext context, IProductRepository productRepository)
        {
            _payPalService = payPalService;
            _context = context;
            _productRepository = productRepository;
            _userManager = userManager;
        }

        public IActionResult CreatePayment()
        {
            string returnUrl = Url.Action("PaymentSuccess", "PayPal", null, Request.Scheme);
            string cancelUrl = Url.Action("PaymentCancel", "PayPal", null, Request.Scheme);

            string approvalUrl = _payPalService.CreatePayment(10.00m, returnUrl, cancelUrl);
            return Redirect(approvalUrl);
        }

        public async Task<IActionResult> PaymentSuccess(string paymentId, string PayerID)
        {
            bool isSuccess = _payPalService.ExecutePayment(paymentId, PayerID);

            if (!isSuccess)
            {
                ViewBag.Message = "Thanh toán thất bại!";
                return View("PaymentFailed");
            }

            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                ViewBag.Message = "Giỏ hàng trống!";
                return View("PaymentFailed");
            }

            // Tạo đơn hàng mới
            var user = await _userManager.GetUserAsync(User);
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.UtcNow,
                OrderDetails = cart.Items.Select(i => new OrderDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList(),
                TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity)
            };
            order.ShippingAddress = HttpContext.Session.GetString("ShippingAddress") ?? "Không có địa chỉ giao hàng";

// Gán ghi chú nếu có
            order.Notes = HttpContext.Session.GetString("OrderNotes") ?? "";
            // Áp dụng giảm giá nếu có
            var discount = HttpContext.Session.GetObjectFromJson<decimal>("DiscountAmount", 0m);
            if (discount > 0)
            {
                order.TotalPrice -= discount;
            }

            _context.Orders.Add(order);

            // Trừ hàng tồn kho
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

            // Xóa giỏ hàng và giảm giá khỏi session
            HttpContext.Session.Remove("Cart");
            HttpContext.Session.Remove("DiscountCode");
            HttpContext.Session.Remove("DiscountAmount");

            ViewBag.Message = "Thanh toán thành công!";
            return View("PaymentSuccess"); // hoặc return View(order); nếu bạn có view hiển thị chi tiết đơn hàng
        }



        public IActionResult PaymentCancel()
        {
            ViewBag.Message = "Thanh toán đã bị hủy.";
            return View();
        }
    }
}