using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DiscountCodeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiscountCodeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var discountCodes = _context.DiscountCodes.ToList();
            return View(discountCodes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DiscountCode discountCode)
        {
            if (ModelState.IsValid)
            {
                discountCode.Code = $"DISCOUNT-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}"; // Tạo mã ngẫu nhiên
                discountCode.UsageCount = 0; // Khởi tạo số lần sử dụng
                _context.DiscountCodes.Add(discountCode);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(discountCode);
        }

        public IActionResult Edit(int id)
        {
            var discountCode = _context.DiscountCodes.Find(id);
            if (discountCode == null)
            {
                return NotFound();
            }
            return View(discountCode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DiscountCode discountCode)
        {
            if (id != discountCode.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Giữ nguyên Code và UsageCount, chỉ cập nhật các trường khác
                    var existingCode = _context.DiscountCodes.AsNoTracking().FirstOrDefault(d => d.Id == id);
                    if (existingCode != null)
                    {
                        discountCode.Code = existingCode.Code;
                        discountCode.UsageCount = existingCode.UsageCount;
                    }
                    _context.Update(discountCode);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Không thể cập nhật mã giảm giá: " + ex.Message);
                }
            }
            return View(discountCode);
        }

        public IActionResult Delete(int id)
        {
            var discountCode = _context.DiscountCodes.Find(id);
            if (discountCode == null)
            {
                return NotFound();
            }
            return View(discountCode);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var discountCode = _context.DiscountCodes.Find(id);
            if (discountCode != null)
            {
                _context.DiscountCodes.Remove(discountCode);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}