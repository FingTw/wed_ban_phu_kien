using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanPhuKienDienThoai.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }

        [Required, Range(0.01, 10000.00)]
        public decimal Price { get; set; }
        public int Stock { get; set; } 

        [ForeignKey("ProductCategory")]
        public int ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; } // Liên kết loại sản phẩm

        public string? ImageUrl { get; set; }
        public List<ProductImage>? Images { get; set; } // Ảnh sản phẩm
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
