using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Repositories
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<DiscountCode>> GetAllAsync();
        Task<DiscountCode?> GetByCodeAsync(string code);
        Task AddAsync(DiscountCode code);
    }

}
