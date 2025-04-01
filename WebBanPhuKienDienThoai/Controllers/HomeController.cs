using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanPhuKienDienThoai.Models;
using WebBanPhuKienDienThoai.Respository;

namespace WebBanPhuKienDienThoai.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
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
            var products = await _productRepository.GetAllAsync();
            var result = products
                .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Select(p => new { p.Id, p.Name, p.ImageUrl })
                .ToList();

            return Json(result);
        }
    }
}