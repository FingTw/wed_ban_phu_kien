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

        public IActionResult CreatePayment()
        {
            string returnUrl = Url.Action("PaymentSuccess", "PayPal", null, Request.Scheme);
            string cancelUrl = Url.Action("PaymentCancel", "PayPal", null, Request.Scheme);

            string approvalUrl = _payPalService.CreatePayment(10.00m, returnUrl, cancelUrl);
            return Redirect(approvalUrl);
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