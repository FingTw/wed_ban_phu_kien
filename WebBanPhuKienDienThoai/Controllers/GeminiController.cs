using Microsoft.AspNetCore.Mvc;
using WebBanPhuKienDienThoai.Services;
using System.Threading.Tasks;

namespace NguyenPhuongTinh_Tuan3.Controllers
{
    public class GeminiController : Controller
    {
        private readonly IGeminiService _geminiAIService;

        public GeminiController(IGeminiService geminiAIService)
        {
            _geminiAIService = geminiAIService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAIResponse(string userInput)
        {
            if (string.IsNullOrEmpty(userInput))
            {
                return Json(new { success = false, error = "Please enter a message." });
            }

            try
            {
                string aiResponse = await _geminiAIService.GetAIResponse(userInput);
                return Json(new { success = true, response = aiResponse });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] string message) // Thêm [FromBody]
        {
            Console.WriteLine($"Received message: '{message}'");
            if (string.IsNullOrEmpty(message))
            {
                return Json(new { success = false, error = "Please enter a message." });
            }

            try
            {
                var aiResponse = await _geminiAIService.GetAIResponse(message);
                return Json(new { success = true, response = aiResponse });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        
    }
}