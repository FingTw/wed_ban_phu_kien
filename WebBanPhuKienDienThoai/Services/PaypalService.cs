using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PayPal.Api;

namespace WebBanPhuKienDienThoai.Services
{
    public class PayPalService
    {
        private readonly IConfiguration _configuration;
        
        public PayPalService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private APIContext GetAPIContext()
        {
            var config = new Dictionary<string, string>
            {
                { "mode", _configuration["PayPal:Mode"] }
            };

            string clientId = _configuration["PayPal:ClientId"];
            string secret = _configuration["PayPal:Secret"];

            var accessToken = new OAuthTokenCredential(clientId, secret, config).GetAccessToken();
            return new APIContext(accessToken) { Config = config };
        }

        public string CreatePayment(decimal amount, string returnUrl, string cancelUrl)
        {
            var apiContext = GetAPIContext();

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        amount = new Amount { total = amount.ToString("F2"), currency = "USD" },
                        description = "Thanh toán đơn hàng"
                    }
                },
                redirect_urls = new RedirectUrls
                {
                    return_url = returnUrl,
                    cancel_url = cancelUrl
                }
            };

            var createdPayment = payment.Create(apiContext);
            return createdPayment.GetApprovalUrl();
        }

        public bool ExecutePayment(string paymentId, string payerId)
        {
            var apiContext = GetAPIContext();
            var payment = new Payment() { id = paymentId };
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            var executedPayment = payment.Execute(apiContext, paymentExecution);

            return executedPayment.state.ToLower() == "approved";
        }
    }
}
