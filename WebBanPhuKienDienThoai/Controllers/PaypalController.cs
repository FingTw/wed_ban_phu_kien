using Microsoft.AspNetCore.Mvc;
using WebBanPhuKienDienThoai.Services;

namespace WebBanPhuKienDienThoai.Controllers
{
    public class PayPalController : Controller
    {
        private readonly PayPalService _payPalService;

        public PayPalController(PayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        [HttpGet]
        public async Task<IActionResult> CreatePayment(int orderId, decimal totalPrice, string returnUrl, string cancelUrl)
        {
            // Tạo payment cho PayPal và lấy approvalUrl
            var paypalOrderId = await _payPalService.CreatePayment(totalPrice, returnUrl, cancelUrl);

            // Chuyển hướng người dùng tới PayPal
            return Redirect($"https://www.paypal.com/checkoutnow?token={paypalOrderId}");
        }

        public IActionResult PaymentSuccess(string paymentId, string PayerID)
        {
            bool isSuccess = _payPalService.ExecutePayment(paymentId, PayerID);
            ViewBag.Message = isSuccess ? "Thanh toán thành công!" : "Thanh toán thất bại!";
            return View();
        }

        public IActionResult PaymentCancel()
        {
            ViewBag.Message = "Thanh toán đã bị hủy.";
            return View();
        }
    }
}