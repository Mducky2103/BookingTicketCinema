using System.Net.Http.Headers;
using System.Text.Json;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IndexModel> _logger; // (Thêm Logger)

        // Model cho các thẻ KPI (Admin)
        [BindProperty]
        public DashboardStatsViewModel? Stats { get; set; }

        // Model cho bảng suất chiếu (Admin & Staff)
        [BindProperty]
        public List<ShowtimeViewModel> TodayShowtimes { get; set; } = new();

        public string? ErrorMessage { get; set; }
        public readonly string ApiBaseUrl; // (Dùng cho Poster)

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<IndexModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            ApiBaseUrl = _configuration["ApiBaseUrl"]!;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            var accessToken = User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            if (string.IsNullOrEmpty(accessToken))
            {
                ErrorMessage = "Lỗi xác thực. Vui lòng đăng nhập lại.";
                return;
            }

            var httpClient = _httpClientFactory.CreateClient("ApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var apiBaseUrl = _configuration["ApiBaseUrl"];

            // --- SỬA LỖI: CHẠY TUẦN TỰ (Sequential) ---

            // 1. Chỉ tải Thống kê nếu là Admin
            if (User.IsInRole("Admin"))
            {
                await LoadDashboardStats(httpClient, apiBaseUrl);
            }

            // 2. Luôn tải Lịch chiếu
            await LoadTodayShowtimes(httpClient, apiBaseUrl);

            // --- HẾT PHẦN SỬA ---
        }

        // Hàm gọi API lấy thống kê (Admin)
        private async Task LoadDashboardStats(HttpClient client, string baseUrl)
        {
            try
            {
                var response = await client.GetAsync($"{baseUrl}/api/statistics/dashboard");
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    Stats = await response.Content.ReadFromJsonAsync<DashboardStatsViewModel>(options);
                }
                else
                {
                    _logger.LogWarning("API /api/statistics/dashboard thất bại, code: {StatusCode}", response.StatusCode);
                    ErrorMessage = "Không thể tải thống kê.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi LoadDashboardStats");
                ErrorMessage = "Lỗi (Stats): " + ex.Message;
            }
        }

        // Hàm gọi API lấy suất chiếu (Admin & Staff)
        private async Task LoadTodayShowtimes(HttpClient client, string baseUrl)
        {
            try
            {
                // (API này chúng ta sẽ tạo ở Bước 2)
                var response = await client.GetAsync($"{baseUrl}/api/showtime/today");
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    TodayShowtimes = await response.Content.ReadFromJsonAsync<List<ShowtimeViewModel>>(options) ?? new();
                }
                else
                {
                    _logger.LogWarning("API /api/showtime/today thất bại, code: {StatusCode}", response.StatusCode);
                    ErrorMessage = "Không thể tải lịch chiếu hôm nay.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi LoadTodayShowtimes");
                ErrorMessage = "Lỗi (Showtimes): " + ex.Message;
            }
        }
    }
}
