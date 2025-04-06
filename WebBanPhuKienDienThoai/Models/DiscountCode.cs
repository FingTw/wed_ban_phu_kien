using System.ComponentModel.DataAnnotations;

namespace WebBanPhuKienDienThoai.Models
{
    public class DiscountCode
    {
        public int Id { get; set; }
        [Required]
        public string Code { get; set; } = null!;
        public decimal? DiscountAmount { get; set; } // Giảm theo tiền
        public double? DiscountPercent { get; set; } // Giảm theo %
        [Required]
        public DateTime ExpiryDate { get; set; }
        public int UsageLimit { get; set; } = 1; // Số lần tối đa mã có thể sử dụng, mặc định là 1
        public int UsageCount { get; set; } = 0; // Số lần mã đã được sử dụng
    }
}