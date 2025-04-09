using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Repositories
{
    public interface IRatingRepository
    {
        Task<IEnumerable<Rating>> GetRatingsByProductIdAsync(int productId);
        Task AddRatingAsync(Rating rating);
    }
}
