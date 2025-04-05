using Microsoft.EntityFrameworkCore;
using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Respository
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            // Lấy tất cả sản phẩm kèm thông tin category
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.DeviceType)  // 🔹 Load thông tin DeviceType
                .Include(p => p.Category)    // 🔹 Load cả Category nếu cần
                .Include(p => p.Images)      // 🔹 Load danh sách hình ảnh
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product , List<ProductImage> productImages)
        {
            _context.Products.Add(product);
            foreach (var image in productImages)
            {
                image.Product = product; // Gắn sản phẩm vào từng hình ảnh
                _context.ProductImages.Add(image);
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchAsync(string query)
        {
            return await _context.Products
                .Where(p => p.Name.Contains(query))
                .ToListAsync();
        }
        public List<Product> SearchProducts(string term)
        {
            return _context.Products
                .Where(p => p.Name.Contains(term))
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl
                })
                .ToList();
        }
        public async Task<IEnumerable<Product>> GetProductsByIdsAsync(int[] productIds)
        {
            return await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .Include(p => p.Category)
                .Include(p => p.DeviceType)
                .Include(p => p.Images)
                .ToListAsync();
        }
    }
}