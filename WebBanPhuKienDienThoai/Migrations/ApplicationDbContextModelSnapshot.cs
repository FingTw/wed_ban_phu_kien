﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebBanPhuKienDienThoai.Models;

#nullable disable

namespace WebBanPhuKienDienThoai.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("WebBanPhuKienDienThoai.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Age")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("WebBanPhuKienDienThoai.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Bảo vệ điện thoại khỏi trầy xước và va đập.",
                            Name = "Ốp lưng"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Kính cường lực bảo vệ màn hình khỏi vỡ.",
                            Name = "Cường lực"
                        },
                        new
                        {
                            Id = 3,
                            Description = "Giải pháp cung cấp năng lượng di động.",
                            Name = "Sạc dự phòng"
                        },
                        new
                        {
                            Id = 4,
                            Description = "Tai nghe Bluetooth tiện lợi.",
                            Name = "Tai nghe không dây"
                        },
                        new
                        {
                            Id = 5,
                            Description = "Dây cáp sạc nhanh, bền bỉ.",
                            Name = "Cáp sạc"
                        },
                        new
                        {
                            Id = 6,
                            Description = "Gậy selfie tiện dụng cho điện thoại.",
                            Name = "Gậy chụp ảnh"
                        },
                        new
                        {
                            Id = 7,
                            Description = "Sạc nhanh không dây tiện lợi.",
                            Name = "Đế sạc không dây"
                        },
                        new
                        {
                            Id = 8,
                            Description = "Giữ ấm tay khi dùng điện thoại mùa lạnh.",
                            Name = "Găng tay cảm ứng"
                        },
                        new
                        {
                            Id = 9,
                            Description = "Hạ nhiệt khi chơi game trên điện thoại.",
                            Name = "Quạt tản nhiệt điện thoại"
                        },
                        new
                        {
                            Id = 10,
                            Description = "Giúp giữ điện thoại ổn định khi xem phim, livestream.",
                            Name = "Giá đỡ điện thoại"
                        });
                });

            modelBuilder.Entity("WebBanPhuKienDienThoai.Models.DeviceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("DeviceTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "Thiết bị laptop",
                            ImageUrl = "laptop.jpg",
                            Name = "Laptop"
                        },
                        new
                        {
                            Id = 2,
                            Description = "Máy tính để bàn",
                            ImageUrl = "pc.jpg",
                            Name = "PC"
                        },
                        new
                        {
                            Id = 3,
                            Description = "Điện thoại Android",
                            ImageUrl = "android.jpg",
                            Name = "Android"
                        },
                        new
                        {
                            Id = 4,
                            Description = "Thiết bị iPhone",
                            ImageUrl = "ios.jpg",
                            Name = "iOS"
                        },
                        new
                        {
                            Id = 5,
                            Description = "Thiết bị iPad",
                            ImageUrl = "ipad.jpg",
                            Name = "iPAD"
                        });
                });

            modelBuilder.Entity("WebBanPhuKienDienThoai.Models.DeviceTypeCategory", b =>
                {
                    b.Property<int>("DeviceTypeId")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.HasKey("DeviceTypeId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("DeviceTypeCategories");

                    b.HasData(
                        new
                        {
                            DeviceTypeId = 1,
                            CategoryId = 1
                        },
                        new
                        {
                            DeviceTypeId = 1,
                            CategoryId = 2
                        },
                        new
                        {
                            DeviceTypeId = 1,
                            CategoryId = 3
                        },
                        new
                        {
                            DeviceTypeId = 1,
                            CategoryId = 4
                        },
                        new
                        {
                            DeviceTypeId = 1,
                            CategoryId = 5
                        },
                        new
                        {
                            DeviceTypeId = 1,
                            CategoryId = 6
                        },
                        new
                        {
                            DeviceTypeId = 1,
                            CategoryId = 7
                        },
                        new
                        {
                            DeviceTypeId = 1,
                            CategoryId = 8
                        },
                        new
                        {
                            DeviceTypeId = 1,
                            CategoryId = 9
                        },
                        new
                        {
                            DeviceTypeId = 1,
                            CategoryId = 10
                        },
                        new
                        {
                            DeviceTypeId = 2,
                            CategoryId = 1
                        },
                        new
                        {
                            DeviceTypeId = 2,
                            CategoryId = 2
                        },
                        new
                        {
                            DeviceTypeId = 2,
                            CategoryId = 3
                        },
                        new
                        {
                            DeviceTypeId = 2,
                            CategoryId = 4
                        },
                        new
                        {
                            DeviceTypeId = 2,
                            CategoryId = 5
                        },
                        new
                        {
                            DeviceTypeId = 2,
                            CategoryId = 6
                        },
                        new
                        {
                            DeviceTypeId = 2,
                            CategoryId = 7
                        },
                        new
                        {
                            DeviceTypeId = 2,
                            CategoryId = 8
                        },
                        new
                        {
                            DeviceTypeId = 2,
                            CategoryId = 9
                        },
                        new
                        {
                            DeviceTypeId = 2,
                            CategoryId = 10
                        },
                        new
                        {
                            DeviceTypeId = 3,
                            CategoryId = 1
                        },
                        new
                        {
                            DeviceTypeId = 3,
                            CategoryId = 2
                        },
                        new
                        {
                            DeviceTypeId = 3,
                            CategoryId = 3
                        },
                        new
                        {
                            DeviceTypeId = 3,
                            CategoryId = 4
                        },
                        new
                        {
                            DeviceTypeId = 3,
                            CategoryId = 5
                        },
                        new
                        {
                            DeviceTypeId = 3,
                            CategoryId = 6
                        },
                        new
                        {
                            DeviceTypeId = 3,
                            CategoryId = 7
                        },
                        new
                        {
                            DeviceTypeId = 3,
                            CategoryId = 8
                        },
                        new
                        {
                            DeviceTypeId = 3,
                            CategoryId = 9
                        },
                        new
                        {
                            DeviceTypeId = 3,
                            CategoryId = 10
                        },
                        new
                        {
                            DeviceTypeId = 4,
                            CategoryId = 1
                        },
                        new
                        {
                            DeviceTypeId = 4,
                            CategoryId = 2
                        },
                        new
                        {
                            DeviceTypeId = 4,
                            CategoryId = 3
                        },
                        new
                        {
                            DeviceTypeId = 4,
                            CategoryId = 4
                        },
                        new
                        {
                            DeviceTypeId = 4,
                            CategoryId = 5
                        },
                        new
                        {
                            DeviceTypeId = 4,
                            CategoryId = 6
                        },
                        new
                        {
                            DeviceTypeId = 4,
                            CategoryId = 7
                        },
                        new
                        {
                            DeviceTypeId = 4,
                            CategoryId = 8
                        },
                        new
                        {
                            DeviceTypeId = 4,
                            CategoryId = 9
                        },
                        new
                        {
                            DeviceTypeId = 4,
                            CategoryId = 10
                        },
                        new
                        {
                            DeviceTypeId = 5,
                            CategoryId = 1
                        },
                        new
                        {
                            DeviceTypeId = 5,
                            CategoryId = 2
                        },
                        new
                        {
                            DeviceTypeId = 5,
                            CategoryId = 3
                        },
                        new
                        {
                            DeviceTypeId = 5,
                            CategoryId = 4
                        },
                        new
                        {
                            DeviceTypeId = 5,
                            CategoryId = 5
                        },
                        new
                        {
                            DeviceTypeId = 5,
                            CategoryId = 6
                        },
                        new
                        {
                            DeviceTypeId = 5,
                            CategoryId = 7
                        },
                        new
                        {
                            DeviceTypeId = 5,
                            CategoryId = 8
                        },
                        new
                        {
                            DeviceTypeId = 5,
                            CategoryId = 9
                        },
                        new
                        {
                            DeviceTypeId = 5,
                            CategoryId = 10
                        });
                });

            modelBuilder.Entity("WebBanPhuKienDienThoai.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("DeviceTypeId")
                        .HasColumnType("int");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("DeviceTypeId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("WebBanPhuKienDienThoai.Models.ProductImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImages");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("WebBanPhuKienDienThoai.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("WebBanPhuKienDienThoai.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebBanPhuKienDienThoai.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("WebBanPhuKienDienThoai.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebBanPhuKienDienThoai.Models.DeviceType", b =>
                {
                    b.HasOne("WebBanPhuKienDienThoai.Models.Category", "Category")
                        .WithMany("DeviceTypes")
                        .HasForeignKey("CategoryId");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("WebBanPhuKienDienThoai.Models.DeviceTypeCategory", b =>
                {
                    b.HasOne("WebBanPhuKienDienThoai.Models.Category", "Category")
                        .WithMany("DeviceTypeCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebBanPhuKienDienThoai.Models.DeviceType", "DeviceType")
                        .WithMany("DeviceTypeCategories")
                        .HasForeignKey("DeviceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("DeviceType");
                });

            modelBuilder.Entity("WebBanPhuKienDienThoai.Models.Product", b =>
                {
                    b.HasOne("WebBanPhuKienDienThoai.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("WebBanPhuKienDienThoai.Models.DeviceType", "DeviceType")
                        .WithMany("Products")
                        .HasForeignKey("DeviceTypeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Category");

                    b.Navigation("DeviceType");
                });

            modelBuilder.Entity("WebBanPhuKienDienThoai.Models.ProductImage", b =>
                {
                    b.HasOne("WebBanPhuKienDienThoai.Models.Product", "Product")
                        .WithMany("Images")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("WebBanPhuKienDienThoai.Models.Category", b =>
                {
                    b.Navigation("DeviceTypeCategories");

                    b.Navigation("DeviceTypes");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("WebBanPhuKienDienThoai.Models.DeviceType", b =>
                {
                    b.Navigation("DeviceTypeCategories");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("WebBanPhuKienDienThoai.Models.Product", b =>
                {
                    b.Navigation("Images");
                });
#pragma warning restore 612, 618
        }
    }
}
