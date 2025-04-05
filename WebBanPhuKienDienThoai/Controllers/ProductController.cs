
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
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDeviceTypeRepository _deviceTypeRepository;

        public ProductController(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IDeviceTypeRepository deviceTypeRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _deviceTypeRepository = deviceTypeRepository;
        }

        public async Task<IActionResult> Index(int? categoryId, int? deviceTypeId, decimal? minPrice, decimal? maxPrice)
        {
            // L?y danh sách Category và DeviceType ?? hi?n th? trong form l?c
            ViewBag.Categories = await _categoryRepository.GetAllAsync();
            ViewBag.DeviceTypes = await _deviceTypeRepository.GetAllAsync();

            // L?y giá min và max ?? thi?t l?p thanh tr??t
            var products = await _productRepository.GetAllAsync();
            ViewBag.MinPrice = products.Any() ? products.Min(p => p.Price) : 0;
            ViewBag.MaxPrice = products.Any() ? products.Max(p => p.Price) : 1000000;

            // L?c s?n ph?m
            var filteredProducts = await _productRepository.FilterAsync(categoryId, deviceTypeId, minPrice, maxPrice);
            return View(filteredProducts);
        }

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
