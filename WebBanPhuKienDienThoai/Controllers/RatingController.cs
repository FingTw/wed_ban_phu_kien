using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Controllers
{
    [Authorize]
    public class RatingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RatingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddRating(int productId, int stars)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            bool alreadyRated = await _context.Ratings.AnyAsync(r => r.ProductId == productId && r.UserId == user.Id);
            if (alreadyRated)
            {
                TempData["Message"] = "Bạn đã đánh giá sản phẩm này rồi.";
                return RedirectToAction("Display", "Product", new { id = productId });
            }

            var rating = new Rating
            {
                UserId = user.Id,
                ProductId = productId,
                Stars = stars,
                CreatedAt = DateTime.UtcNow
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Đánh giá đã được gửi. Cảm ơn bạn!";
            return RedirectToAction("Display", "Product", new { id = productId });
        }

    }
}
