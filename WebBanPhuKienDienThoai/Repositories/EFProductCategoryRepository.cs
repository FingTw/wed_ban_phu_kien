using Microsoft.EntityFrameworkCore;
using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Repositories
{
    public class EFProductCategoryRepository : IProductCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductCategory>> GetAllAsync()
        {
            return await _context.ProductCategories
                .Include(pc => pc.Products) // ✅ Include danh sách Product liên quan
                .ToListAsync();
        }

        public async Task<ProductCategory> GetByIdAsync(int id)
        {
            return await _context.ProductCategories
                .Include(pc => pc.Products) // ✅ Lấy danh sách sản phẩm thuộc danh mục
                .FirstOrDefaultAsync(pc => pc.Id == id);
        }

        public async Task AddAsync(ProductCategory productCategory)
        {
            _context.ProductCategories.Add(productCategory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductCategory productCategory)
        {
            _context.ProductCategories.Update(productCategory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var productCategory = await _context.ProductCategories
                .Include(pc => pc.Products) // ✅ Lấy danh sách sản phẩm thuộc danh mục
                .FirstOrDefaultAsync(pc => pc.Id == id);

            if (productCategory != null)
            {
                // Xóa tất cả sản phẩm liên quan trước khi xóa danh mục
                _context.Products.RemoveRange(productCategory.Products);

                _context.ProductCategories.Remove(productCategory);
                await _context.SaveChangesAsync();
            }
        }
    }
}
