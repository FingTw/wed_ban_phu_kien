using Microsoft.EntityFrameworkCore;
using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Repositories
{
    public class EFDiscountRepository : IDiscountRepository
    {
        private readonly ApplicationDbContext _context;

        public EFDiscountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiscountCode>> GetAllAsync()
        {
            return await _context.DiscountCodes.ToListAsync();
        }

        public async Task<DiscountCode?> GetByIdAsync(int id)
        {
            return await _context.DiscountCodes.FindAsync(id);
        }

        public async Task<DiscountCode?> GetByCodeAsync(string code)
        {
            return await _context.DiscountCodes
                .FirstOrDefaultAsync(d => d.Code == code);
        }

        public async Task AddAsync(DiscountCode code)
        {
            _context.DiscountCodes.Add(code);
            await _context.SaveChangesAsync();
        }

        public void Update(DiscountCode code)
        {
            _context.DiscountCodes.Update(code);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}