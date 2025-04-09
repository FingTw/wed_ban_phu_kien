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

    public async Task<IActionResult> Index(
    int? categoryId,
    int? deviceTypeId,
    decimal? minPrice,
    decimal? maxPrice,
    decimal? priceFilter,
    string sortOrder,
    string searchString)
    {
        // Lấy danh sách Categories và DeviceTypes để hiển thị dropdown filter
        ViewBag.Categories = await _categoryRepository.GetAllAsync();
        ViewBag.DeviceTypes = await _deviceTypeRepository.GetAllAsync();

        // Gán lại giá trị filter vào ViewBag để giữ lại khi postback
        ViewBag.CategoryId = categoryId;
        ViewBag.DeviceTypeId = deviceTypeId;
        ViewBag.SearchString = searchString;
        ViewBag.CurrentFilter = priceFilter;
        ViewBag.SortOrder = sortOrder;

        // Thiết lập min/max cho thanh trượt giá
        ViewBag.MinPrice = 0;
        ViewBag.MaxPrice = 10000000;

        // Bắt đầu truy vấn với tất cả sản phẩm
        var products = await _productRepository.FilterAsync(categoryId, deviceTypeId, minPrice, maxPrice);

        // Lọc theo từ khóa tìm kiếm nếu có
        if (!string.IsNullOrEmpty(searchString))
        {
            products = products
                .Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList(); // chuyển về danh sách để xử lý tiếp
        }

        // Lọc theo giá cụ thể nếu có
        if (priceFilter.HasValue)
        {
            products = products
                .Where(p => p.Price <= priceFilter.Value)
                .ToList();
        }

        // Sắp xếp theo yêu cầu
        switch (sortOrder)
        {
            case "priceAsc":
                products = products.OrderBy(p => p.Price).ToList();
                break;
            case "newest":
                products = products.OrderByDescending(p => p.CreatedAt).ToList();
                break;
        }

        return View(products);
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

