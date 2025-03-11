using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanPhuKienDienThoai.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Url { get; set; } 

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product Product { get; set; } 
    }
}
