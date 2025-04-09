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

        public async Task<IEnumerable<Product>> FilterAsync(int? categoryId, int? deviceTypeId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.DeviceType)
                .Include(p => p.Ratings)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (deviceTypeId.HasValue)
                query = query.Where(p => p.DeviceTypeId == deviceTypeId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            return await query.ToListAsync();
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
                .Include(p => p.Ratings)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.DeviceType)  // 🔹 Load thông tin DeviceType
                .Include(p => p.Category)    // 🔹 Load cả Category nếu cần
                .Include(p => p.Images)      // 🔹 Load danh sách hình ảnh
                .Include(p => p.Ratings)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product, List<ProductImage> productImages)
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
        public async Task<List<Product>> getPaginatedProducts(int pageNumber, int pageSize)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.DeviceType)
                .Include(p => p.Ratings)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryId(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .Include(p => p.DeviceType)
                .ToListAsync();
        }
    }

}