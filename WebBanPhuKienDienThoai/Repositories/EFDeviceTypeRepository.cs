using Microsoft.EntityFrameworkCore;
using WebBanPhuKienDienThoai.Models;

namespace WebBanPhuKienDienThoai.Respository
{
    public class EFDeviceTypeRepository : IDeviceTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public EFDeviceTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DeviceType>> GetAllAsync()
        {
            // Lấy tất cả category, có thể include các thông tin liên quan nếu cần
            return await _context.DeviceTypes.ToListAsync();
        }

        public async Task<DeviceType> GetByIdAsync(int id)
        {
            // Lấy category theo ID
            return await _context.DeviceTypes.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(DeviceType devicetype)
        {
            _context.DeviceTypes.Update(devicetype);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var devicetype = await _context.DeviceTypes.FindAsync(id);
            if (devicetype != null)
            {
                _context.DeviceTypes.Remove(devicetype);
                await _context.SaveChangesAsync();
            }
        }

    }
}