using System.Text.Json;
using System.Text;
using WebBanPhuKienDienThoai.Models;
using Microsoft.EntityFrameworkCore;

namespace WebBanPhuKienDienThoai.Services
{
    public class GeminiService:IGeminiService   
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly ApplicationDbContext _dbContext;

        public GeminiService(HttpClient httpClient, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeminiAI:ApiKey"]; // Giữ nguyên key như code hoạt động
            _apiUrl = configuration["GeminiAI:ApiUrl"];
            _dbContext = dbContext;

            // Kiểm tra cấu hình
            if (string.IsNullOrEmpty(_apiUrl) || !Uri.IsWellFormedUriString(_apiUrl, UriKind.Absolute))
            {
                throw new InvalidOperationException("API URL không hợp lệ. Vui lòng kiểm tra GeminiAI:ApiUrl trong appsettings.json.");
            }
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("API Key không hợp lệ. Vui lòng kiểm tra GeminiAI:ApiKey trong appsettings.json.");
            }
        }

        public async Task<string> GetAIResponse(string input)
        {
            try
            {
                string context = await AnalyzeInputAndGetContext(input);
                var requestBody = new
                {
                    contents = new[]
                    {
                            new
                            {
                                parts = new[]
                                {
                                    new { text = $"Bạn là 1 phần hệ thống của website tôi dựa trên dữ liệu mà tôi cung cấp" +
                                    $"với những sản phẩm không có trong database thì không cần trả lời , nói không bán là được " +
                                    $"khách hàng có thể hỏi bằng tiếng anh " +
                                    $"bạn có thể linh hoạt trả lời các câu hỏi của người dùng . {context}\nCâu hỏi: {input}" }
                                }
                            }
                        }
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var requestUrl = $"{_apiUrl}?key={_apiKey}";
                Console.WriteLine($"Request URL: {requestUrl}");

                var response = await _httpClient.PostAsync(requestUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"API call failed: {response.StatusCode} - {errorContent}");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseString);
                return result.GetProperty("candidates")[0]
                             .GetProperty("content")
                             .GetProperty("parts")[0]
                             .GetProperty("text")
                             .GetString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        private async Task<string> AnalyzeInputAndGetContext(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "Vui lòng nhập câu hỏi!";
            }

            input = input.ToLower();

            // Nếu không dùng database, trả về câu trả lời mặc định
            if (_dbContext == null)
            {
                return "Không có dữ liệu cơ sở dữ liệu để phân tích.";
            }

            if (input.Contains("rẻ nhất") || input.Contains("cheep "))
            {
                var cheapestProduct = await _dbContext.Products
                    .OrderBy(p => p.Price)
                    .FirstOrDefaultAsync();
                if (cheapestProduct != null)
                {
                    return $"Sản phẩm rẻ nhất: {cheapestProduct.Name}, giá {cheapestProduct.Price} USD.";
                }
                return "Không tìm thấy sản phẩm nào.";
            }
            else if (input.Contains("sản phẩm ") || input.Contains("product "))
            {
                var products = await _dbContext.Products.ToListAsync();
                if (products != null && products.Any())
                {
                    return $"Danh sách sản phẩm: {string.Join(", ", products.Select(p => $"{p.Name} - {p.Price} USD"))}";
                }
                return "Không có sản phẩm nào trong cơ sở dữ liệu.";
            }

            return "";
        }
    }
}
