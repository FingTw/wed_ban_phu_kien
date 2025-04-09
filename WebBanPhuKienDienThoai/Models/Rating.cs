namespace WebBanPhuKienDienThoai.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public String UserId { get; set; }
        public int Stars { get; set; } // Rating from 1 to 5
        public string Review { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Product Product { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
