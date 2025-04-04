
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using WebBanPhuKienDienThoai.Models;
using WebBanPhuKienDienThoai.Respository;



namespace WebBanPhuKienDienThoai.Controllers
{
    public class ProductController : Controller 
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index(decimal? priceFilter, string sortOrder, string searchString)
        {
            IEnumerable<Product> products;

            if (!string.IsNullOrEmpty(searchString))
            {
                products = await _productRepository.SearchProductsAsync(searchString);
            }
            else if (priceFilter.HasValue)
            {
                products = await _productRepository.GetProductsByPriceAsync(priceFilter.Value);
                ViewData["CurrentFilter"] = priceFilter.Value;
            }
            else
            {
                products = await _productRepository.GetAllAsync();
            }

            switch (sortOrder)
            {
                case "priceAsc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "newest":
                    products = products.OrderByDescending(p => p.CreatedAt);
                    break;
            }

            return View(products);
        }
        //public async Task<IActionResult> Index()
        //{
        //    var products = await _productRepository.GetAllAsync();
        //    return View(products);
        //}


        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}
