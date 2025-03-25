using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebBanPhuKienDienThoai.Models;
using WebBanPhuKienDienThoai.Repositories;

namespace WebBanPhuKienDienThoai.Areas.Admin.Contronllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // GET: /Admin/Order/Index
        
        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.GetAllAsync();
            return View(orders);
        }

        public IActionResult Add()
        {
            return View();
        }

        // POST: /Admin/Order/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Order order)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    order.OrderDate = DateTime.Now; 
                    await _orderRepository.AddAsync(order);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Không thể thêm đơn hàng: " + ex.Message);
                }
            }
            return View(order);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        // GET: /Admin/Order/Edit/1
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        // POST: /Admin/Order/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _orderRepository.UpdateAsync(order);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Không thể cập nhật đơn hàng: " + ex.Message);
                }
            }
            return View(order);
        }

      

        // GET: /Admin/Order/Delete/1
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        // POST: /Admin/Order/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _orderRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
