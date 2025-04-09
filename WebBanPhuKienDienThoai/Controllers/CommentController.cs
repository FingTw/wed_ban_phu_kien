using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PayPal.Api;
using WebBanPhuKienDienThoai.Models;
using WebBanPhuKienDienThoai.Repositories;

namespace WebBanPhuKienDienThoai.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICommentRepository commentRepository)
        {
            _context = context;
            _userManager = userManager;
            _commentRepository = commentRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int productId, string content)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null)
            {
                return Unauthorized();
            }

            var comment = new Comment
            {
                UserId = user.Id,
                ProductId = productId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Display", "Product", new { id = productId });
        }
    }

}
