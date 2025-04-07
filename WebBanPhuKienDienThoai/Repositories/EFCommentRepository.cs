using Microsoft.EntityFrameworkCore;
using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Repositories
{

    public class EFCommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByProductIdAsync(int productId)
        {
            return await _context.Comments.Where(c => c.ProductId == productId).ToListAsync();
        }

        public async Task AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }
    }
}
