using System.ComponentModel.DataAnnotations;

namespace WebBanPhuKienDienThoai.Models
{
    public class DeviceType
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public List<DeviceTypeCategory>? DeviceTypeCategories { get; set; } = new();
        public List<Product>? Products { get; set; }
        public Category? Category { get; set; }
    }
}
