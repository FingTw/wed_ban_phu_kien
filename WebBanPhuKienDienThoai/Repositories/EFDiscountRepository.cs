using Microsoft.EntityFrameworkCore;
using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Repositories
{
    public class EFDiscountRepository : IDiscountRepository
    {
        private readonly ApplicationDbContext _context;
        public EFDiscountRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<DiscountCode>> GetAllAsync() =>
            await _context.DiscountCodes.ToListAsync();

        public async Task<DiscountCode?> GetByCodeAsync(string code) =>
            await _context.DiscountCodes.FirstOrDefaultAsync(c => c.Code == code);

        public async Task AddAsync(DiscountCode code)
        {
            _context.DiscountCodes.Add(code);
            await _context.SaveChangesAsync();
        }
    }

}
