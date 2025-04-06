using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhuKienDienThoai.Extensions;
using WebBanPhuKienDienThoai.Models;
using System.Linq; // Thêm Linq nếu chưa có
using System.Threading.Tasks; // Thêm Task nếu chưa có

namespace WebBanPhuKienDienThoai.Controllers
{
    [Authorize] // Yêu cầu đăng nhập để truy cập controller này
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        // --- ACTION AddToCart ĐÃ SỬA ---
        [HttpPost] // Chỉ chấp nhận phương thức POST
        [ValidateAntiForgeryToken] // Yêu cầu và kiểm tra AntiForgeryToken
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1) // Nhận productId và quantity (mặc định là 1)
        {
            // Lấy thông tin sản phẩm (nên bao gồm cả ảnh nếu CartItem cần)
            var product = await _productRepository.GetByIdAsync(productId); // Giả sử GetByIdAsync đã include Images nếu cần

            // Kiểm tra sản phẩm và tồn kho
            if (product == null || product.Stock < quantity || quantity <= 0)
            {
                // Trả về lỗi dạng JSON
                return Json(new { success = false, message = "Sản phẩm không tồn tại, số lượng không hợp lệ hoặc không đủ trong kho." });
            }

            // Lấy giỏ hàng từ Session hoặc tạo mới
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            // Tạo đối tượng CartItem
            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price, // Lấy giá từ sản phẩm
                Quantity = quantity, // Giả sử Product model có ImageUrl hoặc lấy từ Images.FirstOrDefault().Url
            };

            // Thêm item vào đối tượng giỏ hàng (logic cộng dồn nếu đã có)
            cart.AddItem(cartItem);

            // Lưu lại giỏ hàng vào Session
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Tính toán tổng số lượng mới trong giỏ hàng
            int newCartCount = cart.GetTotalQuantity(); // Đảm bảo lớp ShoppingCart có phương thức này

            // Trả về JSON chứa kết quả thành công, thông báo và SỐ LƯỢNG MỚI
            return Json(new {
                success = true,
                message = "Đã thêm sản phẩm vào giỏ hàng!", // Thông báo để JavaScript hiển thị
                count = newCartCount // Số lượng mới để JavaScript cập nhật header
            });
        }
        // --- KẾT THÚC SỬA AddToCart ---


        // --- CÁC ACTIONS KHÁC ---
        // Các actions như Index, Checkout, OrderHistory, CompareProducts giữ nguyên

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            // Cập nhật ViewData hoặc ViewBag để layout có thể lấy số lượng ban đầu nếu cần
            ViewData["CartCount"] = cart.GetTotalQuantity();
            return View(cart);
        }

        public async Task<IActionResult> Checkout()
        {
           // Giữ nguyên logic Checkout GET
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
             if (cart == null || !cart.Items.Any()) return RedirectToAction("Index");
             // ... (Logic tạo order và tính toán discount như cũ) ...
             var order = new Order { /* ... */ };
             // ...
             return View(order);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken] // Đảm bảo Checkout POST cũng có ValidateAntiForgeryToken
        public async Task<IActionResult> Checkout(Order order)
        {
            // Giữ nguyên logic Checkout POST
             var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
             if (cart == null || !cart.Items.Any()) return RedirectToAction("Index");
             // ... (Logic lưu order, giảm tồn kho, xóa session như cũ) ...
             var user = await _userManager.GetUserAsync(User);
             order.UserId = user.Id;
             // ...
             _context.Orders.Add(order);
             // ... (cập nhật tồn kho) ...
             await _context.SaveChangesAsync();
             HttpContext.Session.Remove("Cart");
             HttpContext.Session.Remove("DiscountCode");
             return View("OrderCompleted", order.Id);
        }


        [AllowAnonymous]
        public async Task<IActionResult> CompareProducts(string productIds)
        {
            // Giữ nguyên
            if (string.IsNullOrEmpty(productIds)) return View(new List<Product>());
            var ids = productIds.Split(',').Select(int.Parse).ToArray();
            var products = await _productRepository.GetProductsByIdsAsync(ids);
            return View(products);
        }

        public async Task<IActionResult> OrderHistory()
        {
           // Giữ nguyên
           var user = await _userManager.GetUserAsync(User);
           var orders = await _context.Orders /* ... include ... where ... */ .ToListAsync();
           return View(orders);
        }

        // Các action AJAX khác (ApplyDiscount, RemoveFromCart, UpdateCart, ClearCart)
        // Hiện tại chúng đang trả về JSON. Nếu bạn đã chuyển trang giỏ hàng (Index.cshtml)
        // sang dùng form POST và reload trang, thì các action này cũng cần được sửa
        // để return RedirectToAction("Index") và dùng TempData như ở bước trước.
        // Tuy nhiên, nếu bạn chỉ muốn AddToCart dùng AJAX và reload, thì giữ nguyên các action này.
        // Giả sử giữ nguyên các action này trả về JSON cho trang giỏ hàng (nếu nó vẫn dùng AJAX)
[HttpPost]
[ValidateAntiForgeryToken] // Thêm token nếu JS có gửi
public async Task<IActionResult> ApplyDiscount(string discountCode)
{
    // Kiểm tra đầu vào
    if (string.IsNullOrEmpty(discountCode))
    {
        HttpContext.Session.Remove("DiscountCode"); // Xóa mã cũ nếu rỗng
        TempData["DiscountError"] = "Vui lòng nhập mã giảm giá.";
        // Nếu bạn muốn ApplyDiscount reload trang thì dùng Redirect:
        // return RedirectToAction("Index");
        // Nếu ApplyDiscount vẫn dùng AJAX (như code JS hiện tại) thì trả JSON:
        return Json(new { success = false, message = "Vui lòng nhập mã giảm giá." });
    }

    // ---> KHAI BÁO BIẾN 'discount' <---
    var discount = await _context.DiscountCodes
           .FirstOrDefaultAsync(d => d.Code == discountCode && d.ExpiryDate >= DateTime.Now && d.UsageCount < d.UsageLimit);

    // Kiểm tra discount có tồn tại và hợp lệ không
    if (discount == null)
    {
        HttpContext.Session.Remove("DiscountCode");
        TempData["DiscountError"] = "Mã giảm giá không hợp lệ, đã hết hạn hoặc đã dùng hết số lần cho phép.";
        // Nếu reload trang: return RedirectToAction("Index");
        // Nếu AJAX:
        return Json(new { success = false, message = "Mã giảm giá không hợp lệ, đã hết hạn hoặc đã dùng hết số lần cho phép." });
    }

    // Kiểm tra giỏ hàng
    var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
    if (cart == null || !cart.Items.Any())
    {
        // Không cần xóa DiscountCode ở đây vì mã hợp lệ nhưng giỏ hàng trống
        TempData["DiscountError"] = "Giỏ hàng trống, không thể áp dụng mã giảm giá.";
        // Nếu reload trang: return RedirectToAction("Index");
        // Nếu AJAX:
        return Json(new { success = false, message = "Giỏ hàng trống." });
    }

    // Tính toán giá trị giảm giá
    decimal totalPrice = cart.GetTotalPrice(); // Cần phương thức GetTotalPrice() trong ShoppingCart

    // ---> KHAI BÁO BIẾN 'discountAmount' <---
    decimal discountAmount = 0;

    // Kiểm tra loại giảm giá và tính toán (Sử dụng biến 'discount' đã khai báo ở trên)
    // Dòng 147 và 149 trong lỗi của bạn có thể tương ứng với các dòng này
    if (discount.DiscountPercent.HasValue)
    {
        discountAmount = totalPrice * (decimal)(discount.DiscountPercent.Value / 100);
    }
    else if (discount.DiscountAmount.HasValue)
    {
        discountAmount = discount.DiscountAmount.Value;
    }

    // Lưu thông tin giảm giá vào Session và TempData (Sử dụng biến 'discountAmount' đã khai báo ở trên)
    // Dòng 150 trong lỗi của bạn có thể tương ứng với dòng gán TempData["DiscountAmount"]
    HttpContext.Session.SetString("DiscountCode", discount.Code);
    TempData["DiscountAmount"] = discountAmount; // Dùng để hiển thị/tính toán ở View
    TempData["DiscountSuccess"] = $"Đã áp dụng mã giảm giá: {(discount.DiscountPercent.HasValue ? $"{discount.DiscountPercent}%" : $"{discount.DiscountAmount:N0} VNĐ")}"; // Thông báo thành công

    // Nếu reload trang: return RedirectToAction("Index");
    // Nếu AJAX:
    return Json(new { success = true }); // Trả về thành công cho AJAX
}

        [HttpPost]
        [ValidateAntiForgeryToken] // Thêm token nếu JS có gửi
        public IActionResult RemoveFromCart(int productId)
        {
            // Giữ nguyên logic và trả về JSON
             var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
             int newCount = 0;
             if (cart != null)
             {
                 cart.RemoveItem(productId);
                 HttpContext.Session.SetObjectAsJson("Cart", cart);
                 newCount = cart.GetTotalQuantity();
             }
             // Trả về count mới để JS cập nhật (nếu cần)
             return Json(new { success = true, itemCount = newCount });
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart != null)
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == productId);
                if (product == null || quantity > product.Stock)
                {
                    return Json(new { success = false, message = "Số lượng sản phẩm không đủ trong kho." });
                }
                cart.UpdateItem(productId, quantity);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return Json(new { success = true, itemCount = cart?.GetTotalQuantity() ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Thêm token nếu JS có gửi
        public IActionResult ClearCart()
        {
             HttpContext.Session.Remove("Cart");
             return Json(new { success = true, itemCount = 0 }); // Trả về count = 0
        }

    }
}