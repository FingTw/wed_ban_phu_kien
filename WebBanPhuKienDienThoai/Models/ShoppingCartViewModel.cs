namespace WebBanPhuKienDienThoai.Models
{
    public class ShoppingCartViewModel
    {
        public ShoppingCart Cart { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalPrice => TotalPrice - Discount;
    }
}