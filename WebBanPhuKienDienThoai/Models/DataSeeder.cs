using Microsoft.EntityFrameworkCore;
using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Data
{
    public static class DataSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            // Kiểm tra nếu database đã có dữ liệu thì không seed lại
            if (context.Categories.Any() || context.ProductCategories.Any())
            {
                return;
            }

            // Seed dữ liệu cho bảng Category
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Phụ kiện bảo vệ",
                    Description = "Các phụ kiện giúp bảo vệ điện thoại khỏi hư hại."
                },
                new Category
                {
                    Name = "Phụ kiện sạc",
                    Description = "Các phụ kiện hỗ trợ sạc điện thoại."
                },
                new Category
                {
                    Name = "Phụ kiện âm thanh",
                    Description = "Các phụ kiện hỗ trợ âm thanh cho điện thoại."
                },
                new Category
                {
                    Name = "Phụ kiện camera",
                    Description = "Các phụ kiện hỗ trợ chụp ảnh và quay video."
                },
                new Category
                {
                    Name = "Phụ kiện kết nối",
                    Description = "Các phụ kiện hỗ trợ kết nối và mở rộng chức năng."
                },
                new Category
                {
                    Name = "Phụ kiện hỗ trợ sử dụng",
                    Description = "Các phụ kiện giúp sử dụng điện thoại tiện lợi hơn."
                },
                new Category
                {
                    Name = "Phụ kiện đặc biệt",
                    Description = "Các phụ kiện đặc biệt và cao cấp."
                },
                new Category
                {
                    Name = "Phụ kiện làm sạch",
                    Description = "Các phụ kiện giúp vệ sinh điện thoại."
                },
                new Category
                {
                    Name = "Phụ kiện thông minh",
                    Description = "Các phụ kiện thông minh kết nối với điện thoại."
                },
                new Category
                {
                    Name = "Phụ kiện giải trí",
                    Description = "Các phụ kiện hỗ trợ giải trí trên điện thoại."
                }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges(); // Lưu để có ID cho các Category

            // Seed dữ liệu cho bảng ProductCategory
            var productCategories = new List<ProductCategory>
            {
                // Phụ kiện bảo vệ
                new ProductCategory
                {
                    Name = "Ốp lưng",
                    Description = "Ốp lưng bảo vệ điện thoại khỏi trầy xước và va đập.",
                    CategoryId = categories[0].Id
                },
                new ProductCategory
                {
                    Name = "Miếng dán màn hình",
                    Description = "Miếng dán bảo vệ màn hình khỏi trầy xước.",
                    CategoryId = categories[0].Id
                },
                new ProductCategory
                {
                    Name = "Tem camera",
                    Description = "Tem bảo vệ ống kính camera khỏi trầy xước.",
                    CategoryId = categories[0].Id
                },

                // Phụ kiện sạc
                new ProductCategory
                {
                    Name = "Cáp sạc",
                    Description = "Cáp sạc USB, USB-C, Lightning.",
                    CategoryId = categories[1].Id
                },
                new ProductCategory
                {
                    Name = "Sạc dự phòng",
                    Description = "Pin sạc dự phòng cho điện thoại.",
                    CategoryId = categories[1].Id
                },
                new ProductCategory
                {
                    Name = "Sạc không dây",
                    Description = "Sạc không dây tiện lợi.",
                    CategoryId = categories[1].Id
                },

                // Phụ kiện âm thanh
                new ProductCategory
                {
                    Name = "Tai nghe có dây",
                    Description = "Tai nghe có dây chất lượng cao.",
                    CategoryId = categories[2].Id
                },
                new ProductCategory
                {
                    Name = "Tai nghe không dây",
                    Description = "Tai nghe Bluetooth không dây.",
                    CategoryId = categories[2].Id
                },
                new ProductCategory
                {
                    Name = "Loa Bluetooth",
                    Description = "Loa di động kết nối Bluetooth.",
                    CategoryId = categories[2].Id
                },

                // Phụ kiện camera
                new ProductCategory
                {
                    Name = "Ống kính rời",
                    Description = "Ống kính góc rộng, macro, telephoto.",
                    CategoryId = categories[3].Id
                },
                new ProductCategory
                {
                    Name = "Gimbal chống rung",
                    Description = "Gimbal hỗ trợ quay video mượt mà.",
                    CategoryId = categories[3].Id
                },

                // Phụ kiện kết nối
                new ProductCategory
                {
                    Name = "Bộ chia cổng USB",
                    Description = "Bộ chia cổng USB tiện lợi.",
                    CategoryId = categories[4].Id
                },
                new ProductCategory
                {
                    Name = "Thẻ nhớ",
                    Description = "Thẻ nhớ mở rộng bộ nhớ điện thoại.",
                    CategoryId = categories[4].Id
                },

                // Phụ kiện hỗ trợ sử dụng
                new ProductCategory
                {
                    Name = "Bút cảm ứng",
                    Description = "Bút cảm ứng dùng để vẽ hoặc ghi chú.",
                    CategoryId = categories[5].Id
                },
                new ProductCategory
                {
                    Name = "Đế giữ điện thoại",
                    Description = "Đế giữ điện thoại khi xem phim hoặc làm việc.",
                    CategoryId = categories[5].Id
                },

                // Phụ kiện đặc biệt
                new ProductCategory
                {
                    Name = "Máy ảnh nhiệt",
                    Description = "Máy ảnh nhiệt kết nối với điện thoại.",
                    CategoryId = categories[6].Id
                },

                // Phụ kiện làm sạch
                new ProductCategory
                {
                    Name = "Bộ vệ sinh điện thoại",
                    Description = "Bộ dụng cụ vệ sinh điện thoại.",
                    CategoryId = categories[7].Id
                },

                // Phụ kiện thông minh
                new ProductCategory
                {
                    Name = "Đồng hồ thông minh",
                    Description = "Đồng hồ thông minh kết nối với điện thoại.",
                    CategoryId = categories[8].Id
                },

                // Phụ kiện giải trí
                new ProductCategory
                {
                    Name = "Tay cầm chơi game",
                    Description = "Tay cầm chơi game kết nối Bluetooth.",
                    CategoryId = categories[9].Id
                }
            };
            context.ProductCategories.AddRange(productCategories);

            // Lưu thay đổi vào database
            context.SaveChanges();
        }
    }
}