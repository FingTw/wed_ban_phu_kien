using WebBanPhuKienDienThoai.Models;


    public interface IDeviceTypeRepository
    {
        Task<IEnumerable<DeviceType>> GetAllAsync();
        Task<DeviceType> GetByIdAsync(int id);
        Task UpdateAsync(DeviceType devicetype);
        Task DeleteAsync(int id);
    }
