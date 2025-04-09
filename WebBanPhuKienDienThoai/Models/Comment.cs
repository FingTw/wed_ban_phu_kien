namespace WebBanPhuKienDienThoai.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public String UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Product Product { get; set; }
        public virtual ApplicationUser User { get; set; }

     

    }
}
