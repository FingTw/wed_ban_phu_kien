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

        var allProducts = await _productRepository.GetAllAsync();
        var defaultMinPrice = allProducts.Any() ? allProducts.Min(p => p.Price) : 0;
        var defaultMaxPrice = allProducts.Any() ? allProducts.Max(p => p.Price) : 1000000;

        ViewBag.CategoryId = categoryId;
        ViewBag.DeviceTypeId = deviceTypeId;
        ViewBag.MinPrice = defaultMinPrice;
        ViewBag.MaxPrice = defaultMaxPrice;
        ViewBag.SelectedMinPrice = minPrice.HasValue ? minPrice.Value : defaultMinPrice;
        ViewBag.SelectedMaxPrice = maxPrice.HasValue ? maxPrice.Value : defaultMaxPrice;

        var filteredProducts = await _productRepository.FilterAsync(categoryId, deviceTypeId,
            minPrice.HasValue ? minPrice : null,
            maxPrice.HasValue ? maxPrice : null);

        Console.WriteLine($"CategoryId: {categoryId}, DeviceTypeId: {deviceTypeId}, MinPrice: {minPrice}, MaxPrice: {maxPrice}");
        Console.WriteLine($"Filtered products count: {filteredProducts.Count()}");

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
}

