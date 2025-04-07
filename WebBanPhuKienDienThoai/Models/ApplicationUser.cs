using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebBanPhuKienDienThoai.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}