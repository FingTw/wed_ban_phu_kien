using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebBanPhuKienDienThoai.Repositories;
using WebBanPhuKienDienThoai.Models;

public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IDeviceTypeRepository _deviceTypeRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly ICommentRepository _commentRepository;

    public ProductController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IDeviceTypeRepository deviceTypeRepository,
        ICommentRepository commentRepository,
        IRatingRepository ratingRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _deviceTypeRepository = deviceTypeRepository;
        _commentRepository = commentRepository;
        _ratingRepository = ratingRepository;
    }

    public async Task<IActionResult> Index(int? categoryId, int? deviceTypeId, decimal? minPrice, decimal? maxPrice)
    {
        ViewBag.Categories = await _categoryRepository.GetAllAsync();
        ViewBag.DeviceTypes = await _deviceTypeRepository.GetAllAsync();

        ViewBag.CategoryId = categoryId;
        ViewBag.DeviceTypeId = deviceTypeId;

        // L?y giá min và max ?? thi?t l?p thanh tr??t
        ViewBag.MinPrice = 0;
        ViewBag.MaxPrice = 10000000;

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

        product.Comments = (await _commentRepository.GetCommentsByProductIdAsync(id)).ToList();
        product.Ratings = (await _ratingRepository.GetRatingsByProductIdAsync(id)).ToList();

        return View(product);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddComment(int productId, string content)
    {
        var comment = new Comment
        {
            ProductId = productId,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddCommentAsync(comment);

        return RedirectToAction("Display", new { id = productId });
    }

    public async Task<IActionResult> LoadMoreProduct(int page = 1, int pageSize = 6)
    {
        var products = await _productRepository.getPaginatedProducts(page, pageSize);
        if(!products.Any())
        {
            return Content("");
        }    
         return PartialView("_ProductPartial", products);

    }

}

