using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanPhuKienDienThoai.Models;
using WebBanPhuKienDienThoai.Respository;

namespace WebBanPhuKienDienThoai.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class DeviceTypeAController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IDeviceTypeRepository _devicetypeRepository;

        public DeviceTypeAController(IProductRepository productRepository,
                                    ICategoryRepository categoryRepository,
                                    IDeviceTypeRepository devicetypeRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _devicetypeRepository = devicetypeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var devicetype = await _devicetypeRepository.GetAllAsync();
            return View(devicetype);
        }

        public async Task<IActionResult> Display(int id)
        {
            var devicetype = await _devicetypeRepository.GetByIdAsync(id);
            if (devicetype == null)
            {
                return NotFound();
            }
            return View(devicetype);
        }


        // Cho phép Admin và Employee sửa danh mục
        public async Task<IActionResult> Update(int id)
        {
            var devicetype = await _devicetypeRepository.GetByIdAsync(id);
            if (devicetype == null)
            {
                return NotFound();
            }
            return View(devicetype);
        }

        // Cho phép Admin và Employee sửa danh mục
        [HttpPost]
        public async Task<IActionResult> Update(int id, DeviceType devicetype)
        {
            if (id != devicetype.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _devicetypeRepository.UpdateAsync(devicetype);
                return RedirectToAction(nameof(Index));
            }
            return View(devicetype);
        }

        // Cho phép Admin và Employee xóa danh mục
        public async Task<IActionResult> Delete(int id)
        {
            var devicetype = await _devicetypeRepository.GetByIdAsync(id);
            if (devicetype == null)
            {
                return NotFound();
            }
            return View(devicetype);
        }

        // Cho phép Admin và Employee xóa danh mục
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var devicetype = await _devicetypeRepository.GetByIdAsync(id);
            if (devicetype != null)
            {
                await _devicetypeRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
