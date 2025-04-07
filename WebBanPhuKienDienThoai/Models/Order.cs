using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanPhuKienDienThoai.Models
{
    public class Order
    {
        public string? ImageUrl { get; set; }
        public List<ProductImage>? Images { get; set; } = new List<ProductImage>();
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalPrice { get; set; }
        public string ShippingAddress { get; set; } = null!;
        public string? Notes { get; set; }
        public string? PaymentMethod { get; set; }
        public string Status { get; set; } = "Pending";

        // Discount fields
        public int? DiscountCodeId { get; set; }
        public decimal DiscountAmount { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;

        [ForeignKey("DiscountCodeId")]
        public DiscountCode? DiscountCode { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}