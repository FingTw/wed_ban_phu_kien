using WebBanPhuKienDienThoai.Models;

    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product , List<ProductImage> productImages);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<IEnumerable<Product>> GetByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Product>> GetProductsByPriceAsync(decimal price);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchString);
}
