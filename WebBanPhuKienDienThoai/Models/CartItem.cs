
namespace WebBanPhuKienDienThoai.Models
{
    public class CartItem
    {
        public Product? Product { get; set; }

        public string? ImageUrl { get; set; }
        public List<ProductImage>? Images { get; set; } = new List<ProductImage>();
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
