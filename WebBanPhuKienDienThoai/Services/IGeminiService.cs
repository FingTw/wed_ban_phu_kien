namespace WebBanPhuKienDienThoai.Services
{
    public interface IGeminiService
    {
        Task<string> GetAIResponse(string input);
    }
}
