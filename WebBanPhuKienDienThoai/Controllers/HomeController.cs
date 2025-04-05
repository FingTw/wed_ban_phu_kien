using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanPhuKienDienThoai.Models;
using WebBanPhuKienDienThoai.Respository;
using Microsoft.Extensions.Localization;// Thêm cái này
using Microsoft.AspNetCore.Localization;// Thêm cái này


namespace WebBanPhuKienDienThoai.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IStringLocalizer<HomeController> _localizer; // Thêm dòng này

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, IStringLocalizer<HomeController> localizer)
        {
            _logger = logger;
            _productRepository = productRepository;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            var LocalizedTitle = _localizer["Title"];// thêm dòng này
            var products = await _productRepository.GetAllAsync();  
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new List<object>()); // Trả về danh sách rỗng nếu query null hoặc rỗng
            }

            var products = await _productRepository.GetAllAsync();

            var result = products
                .Where(p => p.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(p => new { p.Id, p.Name, p.ImageUrl })
                .ToList();

            return Json(result);
        }

        // Thêm đoạn này để lựa chọn ngôn ngữ
        [HttpPost]
        public IActionResult ChangeLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

    }
}