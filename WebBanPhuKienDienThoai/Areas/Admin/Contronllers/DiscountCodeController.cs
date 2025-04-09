using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Areas.Admin.Controllers
{
    [Area("Admin")]

    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class DiscountCodeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiscountCodeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/DiscountCode
        public async Task<IActionResult> Index()
        {
            var discountCodes = await _context.DiscountCodes
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
            return View(discountCodes);
        }

        // GET: Admin/DiscountCode/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discountCode = await _context.DiscountCodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discountCode == null)
            {
                return NotFound();
            }

            return View(discountCode);
        }

        // GET: Admin/DiscountCode/Create
        public IActionResult Create()
        {
            ViewBag.GeneratedCode = GenerateRandomCode();
            return View();
        }

        // POST: Admin/DiscountCode/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiscountCode discountCode)
        {
            // Kiểm tra mã không được trống
            if (string.IsNullOrEmpty(discountCode.Code))
            {
                ModelState.AddModelError("Code", "Vui lòng nhập mã giảm giá");
                ViewBag.GeneratedCode = GenerateRandomCode();
                return View(discountCode);
            }

            // Kiểm tra chỉ một loại giảm giá được nhập
            if ((discountCode.DiscountPercent.HasValue && discountCode.DiscountAmount.HasValue) ||
                (!discountCode.DiscountPercent.HasValue && !discountCode.DiscountAmount.HasValue))
            {
                ModelState.AddModelError("", "Vui lòng chọn một loại giảm giá: phần trăm hoặc số tiền cố định.");
                ViewBag.GeneratedCode = discountCode.Code ?? GenerateRandomCode();
                return View(discountCode);
            }

            if (ModelState.IsValid)
            {
                // Check if code already exists
                if (await _context.DiscountCodes.AnyAsync(d => d.Code == discountCode.Code))
                {
                    ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại.");
                    ViewBag.GeneratedCode = GenerateRandomCode();
                    return View(discountCode);
                }

                discountCode.UsageCount = 0;

                _context.Add(discountCode);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo mã giảm giá thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.GeneratedCode = discountCode.Code ?? GenerateRandomCode();
            return View(discountCode);
        }

        // GET: Admin/DiscountCode/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discountCode = await _context.DiscountCodes.FindAsync(id);
            if (discountCode == null)
            {
                return NotFound();
            }
            return View(discountCode);
        }

        // POST: Admin/DiscountCode/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DiscountCode discountCode)
        {
            if (id != discountCode.Id)
            {
                return NotFound();
            }

            // Kiểm tra chỉ một loại giảm giá được nhập
            if ((discountCode.DiscountPercent.HasValue && discountCode.DiscountAmount.HasValue) ||
                (!discountCode.DiscountPercent.HasValue && !discountCode.DiscountAmount.HasValue))
            {
                ModelState.AddModelError("", "Vui lòng chọn một loại giảm giá: phần trăm hoặc số tiền cố định.");
                return View(discountCode);
            }

            // Lấy thông tin mã giảm giá hiện tại
            var existingCode = await _context.DiscountCodes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            if (existingCode == null)
            {
                return NotFound();
            }

            // Giữ nguyên các thông tin quan trọng
            discountCode.Code = existingCode.Code;
            discountCode.UsageCount = existingCode.UsageCount;
            discountCode.CreatedAt = existingCode.CreatedAt;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discountCode);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật mã giảm giá thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscountCodeExists(discountCode.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(discountCode);
        }

        // GET: Admin/DiscountCode/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discountCode = await _context.DiscountCodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discountCode == null)
            {
                return NotFound();
            }

            return View(discountCode);
        }

        // POST: Admin/DiscountCode/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discountCode = await _context.DiscountCodes.FindAsync(id);
            if (discountCode == null)
            {
                return NotFound();
            }

            _context.DiscountCodes.Remove(discountCode);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa mã giảm giá thành công!";
            return RedirectToAction(nameof(Index));
        }

        private bool DiscountCodeExists(int id)
        {
            return _context.DiscountCodes.Any(e => e.Id == id);
        }

        private string GenerateRandomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpGet]
        public IActionResult GenerateCode()
        {
            return Json(new { code = GenerateRandomCode() });
        }
    }
}
