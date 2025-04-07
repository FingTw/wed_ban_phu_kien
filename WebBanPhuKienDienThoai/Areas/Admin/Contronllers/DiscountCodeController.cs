using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiscountCodeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiscountCodeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiscountCode discountCode)
        {
            if (ModelState.IsValid)
            {
                _context.DiscountCodes.Add(discountCode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(discountCode);
        }

        public async Task<IActionResult> Index()
        {
            var discountCodes = await _context.DiscountCodes.ToListAsync();
            return View(discountCodes);
        }
    }
}
