
namespace WebBanPhuKienDienThoai.Models
{
    public class DeviceTypeCategory
    {
        public int DeviceTypeId { get; set; }
        public DeviceType DeviceType { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}