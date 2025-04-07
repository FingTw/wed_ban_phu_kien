using Microsoft.AspNetCore.Identity;
using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Services
{
    public class DataInitializer
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed roles
            string[] roleNames = { SD.Role_Admin, SD.Role_Employee, SD.Role_Customer, SD.Role_Company };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed users
            await CreateUser(userManager, "admin1@example.com", "Admin@123", SD.Role_Admin, "Admin One");
            await CreateUser(userManager, "admin2@example.com", "Admin@123", SD.Role_Admin, "Admin Two");
            await CreateUser(userManager, "employee1@example.com", "Employee@123", SD.Role_Employee, "Employee One");
            await CreateUser(userManager, "employee2@example.com", "Employee@123", SD.Role_Employee, "Employee Two");
            await CreateUser(userManager, "customer1@example.com", "Customer@123", SD.Role_Customer, "Customer One");
            await CreateUser(userManager, "customer2@example.com", "Customer@123", SD.Role_Customer, "Customer Two");
            await CreateUser(userManager, "company1@example.com", "Company@123", SD.Role_Company, "Company One");
            await CreateUser(userManager, "company2@example.com", "Company@123", SD.Role_Company, "Company Two");

            // Seed products
            if (!context.Products.Any())
            {
                var random = new Random();
                var products = new List<Product>();

                for (int i = 1; i <= 50; i++)
                {
                    products.Add(new Product
                    {
                        Name = $"Product {i}",
                        Price = random.Next(10000, 1000000),
                        Description = $"Description for product {i}",
                        Stock = random.Next(1, 100),
                        ImageUrl = "~/images/iphone-16-blue-600x600.png",
                        CategoryId = random.Next(1, 11), 
                        DeviceTypeId = random.Next(1, 6),
                        CreatedAt = DateTime.UtcNow
                    });
                }

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }

        private static async Task CreateUser(UserManager<ApplicationUser> userManager, string email, string password, string role, string fullName)
        {
            if (userManager.Users.All(u => u.Email != email))
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
