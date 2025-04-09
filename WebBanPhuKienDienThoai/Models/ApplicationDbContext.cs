using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebBanPhuKienDienThoai.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<DeviceTypeCategory> DeviceTypeCategories { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.DeviceType)
                .WithMany(dt => dt.Products)
                .HasForeignKey(p => p.DeviceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DeviceTypeCategory>()
                .HasKey(dc => new { dc.DeviceTypeId, dc.CategoryId });

            modelBuilder.Entity<DeviceTypeCategory>()
                .HasOne(dc => dc.DeviceType)
                .WithMany(d => d.DeviceTypeCategories)
                .HasForeignKey(dc => dc.DeviceTypeId);

            modelBuilder.Entity<DeviceTypeCategory>()
                .HasOne(dc => dc.Category)
                .WithMany(c => c.DeviceTypeCategories)
                .HasForeignKey(dc => dc.CategoryId);

            modelBuilder.Entity<Comment>()
           .HasOne(c => c.Product)
           .WithMany(p => p.Comments)
           .HasForeignKey(c => c.ProductId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Ratings)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Ốp lưng", Description = "Bảo vệ điện thoại khỏi trầy xước và va đập." },
                new Category { Id = 2, Name = "Cường lực", Description = "Kính cường lực bảo vệ màn hình khỏi vỡ." },
                new Category { Id = 3, Name = "Sạc dự phòng", Description = "Giải pháp cung cấp năng lượng di động." },
                new Category { Id = 4, Name = "Tai nghe không dây", Description = "Tai nghe Bluetooth tiện lợi." },
                new Category { Id = 5, Name = "Cáp sạc", Description = "Dây cáp sạc nhanh, bền bỉ." },
                new Category { Id = 6, Name = "Gậy chụp ảnh", Description = "Gậy selfie tiện dụng cho điện thoại." },
                new Category { Id = 7, Name = "Đế sạc không dây", Description = "Sạc nhanh không dây tiện lợi." },
                new Category { Id = 8, Name = "Găng tay cảm ứng", Description = "Giữ ấm tay khi dùng điện thoại mùa lạnh." },
                new Category { Id = 9, Name = "Quạt tản nhiệt điện thoại", Description = "Hạ nhiệt khi chơi game trên điện thoại." },
                new Category { Id = 10, Name = "Giá đỡ điện thoại", Description = "Giúp giữ điện thoại ổn định khi xem phim, livestream." }
            );

            modelBuilder.Entity<DeviceType>().HasData(
                new DeviceType { Id = 1, Name = "Laptop", Description = "Thiết bị laptop", ImageUrl = "laptop.jpg" },
                new DeviceType { Id = 2, Name = "PC", Description = "Máy tính để bàn", ImageUrl = "pc.jpg" },
                new DeviceType { Id = 3, Name = "Android", Description = "Điện thoại Android", ImageUrl = "android.jpg" },
                new DeviceType { Id = 4, Name = "iOS", Description = "Thiết bị iPhone", ImageUrl = "ios.jpg" },
                new DeviceType { Id = 5, Name = "iPAD", Description = "Thiết bị iPad", ImageUrl = "ipad.jpg" }
            );

            modelBuilder.Entity<DeviceTypeCategory>().HasData(
                new DeviceTypeCategory { DeviceTypeId = 1, CategoryId = 1 },
                new DeviceTypeCategory { DeviceTypeId = 1, CategoryId = 2 },
                new DeviceTypeCategory { DeviceTypeId = 1, CategoryId = 3 },
                new DeviceTypeCategory { DeviceTypeId = 1, CategoryId = 4 },
                new DeviceTypeCategory { DeviceTypeId = 1, CategoryId = 5 },
                new DeviceTypeCategory { DeviceTypeId = 1, CategoryId = 6 },
                new DeviceTypeCategory { DeviceTypeId = 1, CategoryId = 7 },
                new DeviceTypeCategory { DeviceTypeId = 1, CategoryId = 8 },
                new DeviceTypeCategory { DeviceTypeId = 1, CategoryId = 9 },
                new DeviceTypeCategory { DeviceTypeId = 1, CategoryId = 10 },
                new DeviceTypeCategory { DeviceTypeId = 2, CategoryId = 1 },
                new DeviceTypeCategory { DeviceTypeId = 2, CategoryId = 2 },
                new DeviceTypeCategory { DeviceTypeId = 2, CategoryId = 3 },
                new DeviceTypeCategory { DeviceTypeId = 2, CategoryId = 4 },
                new DeviceTypeCategory { DeviceTypeId = 2, CategoryId = 5 },
                new DeviceTypeCategory { DeviceTypeId = 2, CategoryId = 6 },
                new DeviceTypeCategory { DeviceTypeId = 2, CategoryId = 7 },
                new DeviceTypeCategory { DeviceTypeId = 2, CategoryId = 8 },
                new DeviceTypeCategory { DeviceTypeId = 2, CategoryId = 9 },
                new DeviceTypeCategory { DeviceTypeId = 2, CategoryId = 10 },
                new DeviceTypeCategory { DeviceTypeId = 3, CategoryId = 1 },
                new DeviceTypeCategory { DeviceTypeId = 3, CategoryId = 2 },
                new DeviceTypeCategory { DeviceTypeId = 3, CategoryId = 3 },
                new DeviceTypeCategory { DeviceTypeId = 3, CategoryId = 4 },
                new DeviceTypeCategory { DeviceTypeId = 3, CategoryId = 5 },
                new DeviceTypeCategory { DeviceTypeId = 3, CategoryId = 6 },
                new DeviceTypeCategory { DeviceTypeId = 3, CategoryId = 7 },
                new DeviceTypeCategory { DeviceTypeId = 3, CategoryId = 8 },
                new DeviceTypeCategory { DeviceTypeId = 3, CategoryId = 9 },
                new DeviceTypeCategory { DeviceTypeId = 3, CategoryId = 10 },
                new DeviceTypeCategory { DeviceTypeId = 4, CategoryId = 1 },
                new DeviceTypeCategory { DeviceTypeId = 4, CategoryId = 2 },
                new DeviceTypeCategory { DeviceTypeId = 4, CategoryId = 3 },
                new DeviceTypeCategory { DeviceTypeId = 4, CategoryId = 4 },
                new DeviceTypeCategory { DeviceTypeId = 4, CategoryId = 5 },
                new DeviceTypeCategory { DeviceTypeId = 4, CategoryId = 6 },
                new DeviceTypeCategory { DeviceTypeId = 4, CategoryId = 7 },
                new DeviceTypeCategory { DeviceTypeId = 4, CategoryId = 8 },
                new DeviceTypeCategory { DeviceTypeId = 4, CategoryId = 9 },
                new DeviceTypeCategory { DeviceTypeId = 4, CategoryId = 10 },
                new DeviceTypeCategory { DeviceTypeId = 5, CategoryId = 1 },
                new DeviceTypeCategory { DeviceTypeId = 5, CategoryId = 2 },
                new DeviceTypeCategory { DeviceTypeId = 5, CategoryId = 3 },
                new DeviceTypeCategory { DeviceTypeId = 5, CategoryId = 4 },
                new DeviceTypeCategory { DeviceTypeId = 5, CategoryId = 5 },
                new DeviceTypeCategory { DeviceTypeId = 5, CategoryId = 6 },
                new DeviceTypeCategory { DeviceTypeId = 5, CategoryId = 7 },
                new DeviceTypeCategory { DeviceTypeId = 5, CategoryId = 8 },
                new DeviceTypeCategory { DeviceTypeId = 5, CategoryId = 9 },
                new DeviceTypeCategory { DeviceTypeId = 5, CategoryId = 10 }
            );

        }
    }
}