using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using WebBanPhuKienDienThoai.Models;
using WebBanPhuKienDienThoai.Repositories;

namespace WebBanPhuKienDienThoai.Areas.Admin.Controllers
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

        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.GetAllAsync();
            return View(orders);
        }

        public IActionResult Add()
        {
            return View();
        }

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

        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

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

        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _orderRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportInvoice(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return NotFound();

            using (var memoryStream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(memoryStream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Tiêu đề
                document.Add(new Paragraph("Hóa Đơn - Phụ Kiện Điện Thoại")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20));


                // Thông tin đơn hàng
                document.Add(new Paragraph($"Mã đơn hàng: {order.Id}"));
                document.Add(new Paragraph($"Khách hàng: {order.ApplicationUser?.UserName}"));
                document.Add(new Paragraph($"Ngày đặt: {order.OrderDate:dd/MM/yyyy HH:mm}"));
                document.Add(new Paragraph($"Địa chỉ giao hàng: {order.ShippingAddress}"));
                if (!string.IsNullOrEmpty(order.Notes))
                    document.Add(new Paragraph($"Ghi chú: {order.Notes}"));

                document.Add(new Paragraph(" ")); // Khoảng cách

                // Bảng chi tiết sản phẩm
                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 10, 40, 15, 20, 20 }));
                table.SetWidth(UnitValue.CreatePercentValue(100));

                table.AddHeaderCell(new Cell().Add(new Paragraph("STT")).SetTextAlignment(TextAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Sản phẩm")).SetTextAlignment(TextAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Số lượng")).SetTextAlignment(TextAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Đơn giá")).SetTextAlignment(TextAlignment.CENTER));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Thành tiền")).SetTextAlignment(TextAlignment.CENTER));

                int index = 1;
                foreach (var detail in order.OrderDetails)
                {
                    table.AddCell(new Cell().Add(new Paragraph(index++.ToString())).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(detail.Product?.Name ?? "Không xác định")));
                    table.AddCell(new Cell().Add(new Paragraph(detail.Quantity.ToString())).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph($"{detail.Price:#,##0} VNĐ")).SetTextAlignment(TextAlignment.RIGHT));
                    table.AddCell(new Cell().Add(new Paragraph($"{(detail.Price * detail.Quantity):#,##0} VNĐ")).SetTextAlignment(TextAlignment.RIGHT));
                }

                document.Add(table);

                // Tổng tiền
                document.Add(new Paragraph($"Tổng tiền: {order.TotalPrice:#,##0} VNĐ")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(14));
                    

                document.Close();

                byte[] pdfBytes = memoryStream.ToArray();
                return File(pdfBytes, "application/pdf", $"HoaDon_{order.Id}.pdf");
            }
        }
    }
}