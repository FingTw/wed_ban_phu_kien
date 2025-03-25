
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using WebBanPhuKienDienThoai.Models;
using WebBanPhuKienDienThoai.Respository;



namespace WebBanPhuKienDienThoai.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductAController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDeviceTypeRepository _devicetypeRepository;

        public ProductAController(IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IDeviceTypeRepository devicetypeRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _devicetypeRepository = devicetypeRepository;
        }

        private void LogToFile(string message)
        {
            var logPath = "log.txt";
            System.IO.File.AppendAllText(logPath, DateTime.Now + " - " + message + "\n");
        }
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            var devicetypes = await _devicetypeRepository.GetAllAsync();
            ViewBag.DeviceTypes = new SelectList(devicetypes, "Id", "Name");
            return View();
        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Product product, List<IFormFile> imageUrls , IFormFile imageUrl)
        {
            if (!ModelState.IsValid)
            {
                // Xử lý lỗi validation
                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                var devicetypes = await _devicetypeRepository.GetAllAsync();
                ViewBag.DeviceTypes = new SelectList(devicetypes, "Id", "Name");
                return View(product);
            }

            var productImages = new List<ProductImage>();

            // Xử lý hình ảnh chính
            if (imageUrl != null && imageUrl.Length > 0)
            {
                product.ImageUrl = await SaveImage(imageUrl);
            }

            // Xử lý hình ảnh bổ sung
            if (imageUrls != null && imageUrls.Count > 0)
            {
                foreach (var image in imageUrls)
                {
                    var imageUrlPath = await SaveImage(image);
                    productImages.Add(new ProductImage { Url = imageUrlPath });
                }
            }

            await _productRepository.AddAsync(product, productImages);
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName;
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

        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            var devicetypes = await _devicetypeRepository.GetAllAsync();
            ViewBag.DeviceTypes = new SelectList(devicetypes, "Id", "Name", product.DeviceTypeId);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
            if (id != product.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetByIdAsync(id);
                // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                if (imageUrl == null)
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                else
                {
                    // Lưu hình ảnh mới
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                // Cập nhật các thông tin khác của sản phẩm
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.DeviceTypeId = product.DeviceTypeId;
                existingProduct.Stock = product.Stock;
                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(Index));
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            var devicetypes = await _devicetypeRepository.GetAllAsync();
            ViewBag.DeviceTypes = new SelectList(devicetypes, "Id", "Name");
            return View(product);
        }

        // Hiển thị form xác nhận xóa sản phẩm (Chỉ Admin và Employee có quyền)
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
