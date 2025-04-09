using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Repositories
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetCommentsByProductIdAsync(int productId);
        Task AddCommentAsync(Comment comment);
    }
}
