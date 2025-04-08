using Microsoft.Extensions.Options;
using PayPal.Api;
using WebBanPhuKienDienThoai.Models;

public class PayPalService
{
    private readonly PayPalConfig _payPalConfig;

    public PayPalService(IOptions<PayPalConfig> payPalConfig)
    {
        _payPalConfig = payPalConfig.Value;
    }

    public async Task<string> CreatePayment(decimal totalPrice, string returnUrl, string cancelUrl)
    {
        var config = new Dictionary<string, string> { { "mode", _payPalConfig.Mode } };
        var accessToken = new OAuthTokenCredential(_payPalConfig.ClientId, _payPalConfig.ClientSecret, config).GetAccessToken();
        var apiContext = new APIContext(accessToken) { Config = config };

        var payer = new Payer() { payment_method = "paypal" };
        var redirectUrls = new RedirectUrls()
        {
            cancel_url = cancelUrl,
            return_url = returnUrl
        };

        var transactionList = new List<Transaction>
    {
        new Transaction()
        {
            amount = new Amount() { currency = "USD", total = totalPrice.ToString("F2") },
            description = $"Thanh toán đơn hàng"
        }
    };

        var payment = new Payment()
        {
            intent = "sale",
            payer = payer,
            transactions = transactionList,
            redirect_urls = redirectUrls
        };

        var createdPayment = await Task.Run(() => payment.Create(apiContext)); // Đẩy vào Task.Run
        var approvalUrl = createdPayment.links.First(l => l.rel.ToLower() == "approval_url").href;

        return approvalUrl;
    }
    public bool ExecutePayment(string paymentId, string payerId)
    {
        var config = new Dictionary<string, string> { { "mode", _payPalConfig.Mode } };
        var accessToken = new OAuthTokenCredential(_payPalConfig.ClientId, _payPalConfig.ClientSecret, config).GetAccessToken();
        var apiContext = new APIContext(accessToken) { Config = config };

        var payment = new Payment() { id = paymentId };
        var executedPayment = payment.Execute(apiContext, new PaymentExecution() { payer_id = payerId });

        return executedPayment.state.ToLower() == "approved";
    }
}
