using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Repositories
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<DiscountCode>> GetAllAsync();
        Task<DiscountCode?> GetByIdAsync(int id);
        Task<DiscountCode?> GetByCodeAsync(string code);
        Task AddAsync(DiscountCode code);
        void Update(DiscountCode code);
        Task SaveChangesAsync();
    }
}
