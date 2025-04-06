using System.ComponentModel.DataAnnotations;

namespace WebBanPhuKienDienThoai.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Range(1000, 1000000000)]
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public List<ProductImage>? Images { get; set; } = new List<ProductImage>();
        public int? CategoryId { get; set; }
        public int? DeviceTypeId { get; set; }
        public DeviceType? DeviceType { get; set; }
        public Category? Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}