using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBanPhuKienDienThoai.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }

        public List<ProductCategory> ProductCategories { get; set; } = new();
    }
}
