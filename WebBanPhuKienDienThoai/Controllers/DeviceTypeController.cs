using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanPhuKienDienThoai.Models;
using WebBanPhuKienDienThoai.Respository;

namespace WebBanPhuKienDienThoai.Controllers
{

    public class DeviceTypeController : Controller
    {
        private readonly IDeviceTypeRepository _devicetypeRepository;

        public DeviceTypeController(IDeviceTypeRepository devicetypeRepository)
        {
            
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

    }
}
