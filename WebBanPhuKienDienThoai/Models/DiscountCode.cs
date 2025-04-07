namespace WebBanPhuKienDienThoai.Models
{
    public class DiscountCode
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public decimal? DiscountAmount { get; set; } // Giảm theo tiền
        public double? DiscountPercent { get; set; } // Giảm theo %
        public DateTime ExpiryDate { get; set; }
        public int UsageLimit { get; set; } = 1;
        public int UsageCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}