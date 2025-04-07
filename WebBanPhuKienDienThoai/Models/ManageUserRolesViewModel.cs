namespace WebBanPhuKienDienThoai.Models
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<UserRoleViewModel> Roles { get; set; }
    }
}
