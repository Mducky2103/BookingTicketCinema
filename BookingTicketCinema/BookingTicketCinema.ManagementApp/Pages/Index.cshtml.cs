using System.Net.Http.Headers;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages
{
    [Authorize(Policy = "RequireStaff")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        // Model cho các thẻ KPI (Admin)
        [BindProperty]
        public DashboardStatsViewModel? Stats { get; set; }

        // Model cho bảng suất chiếu (Admin & Staff)
        [BindProperty]
        public List<ShowtimeViewModel> TodayShowtimes { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            // Lấy token từ phiên đăng nhập (cookie)
            var accessToken = User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            if (string.IsNullOrEmpty(accessToken))
            {
                ErrorMessage = "Lỗi xác thực. Vui lòng đăng nhập lại.";
                return; // Sẽ bị chặn bởi [Authorize] nhưng cẩn thận vẫn hơn
            }

            var httpClient = _httpClientFactory.CreateClient("ApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var apiBaseUrl = _configuration["ApiBaseUrl"];

            var tasks = new List<Task>();

            // === TẢI DỮ LIỆU ===

            // 1. Chỉ tải Thống kê nếu là Admin
            if (User.IsInRole("Admin"))
            {
                // Giả định bạn có endpoint này ở API
                tasks.Add(LoadDashboardStats(httpClient, apiBaseUrl));
            }

            // 2. Luôn tải Lịch chiếu
            // Giả định bạn có endpoint này ở API
            tasks.Add(LoadTodayShowtimes(httpClient, apiBaseUrl));

            await Task.WhenAll(tasks);
        }

        // Hàm gọi API lấy thống kê (Admin)
        private async Task LoadDashboardStats(HttpClient client, string baseUrl)
        {
            try
            {
                var response = await client.GetAsync($"{baseUrl}/api/statistics/dashboard");
                if (response.IsSuccessStatusCode)
                {
                    Stats = await response.Content.ReadFromJsonAsync<DashboardStatsViewModel>();
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                ErrorMessage = "Không thể tải thống kê.";
            }
        }

        // Hàm gọi API lấy suất chiếu (Admin & Staff)
        private async Task LoadTodayShowtimes(HttpClient client, string baseUrl)
        {
            try
            {
                var response = await client.GetAsync($"{baseUrl}/api/showtimes/today");
                if (response.IsSuccessStatusCode)
                {
                    TodayShowtimes = await response.Content.ReadFromJsonAsync<List<ShowtimeViewModel>>() ?? new();
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                ErrorMessage = "Không thể tải lịch chiếu.";
            }
        }
    }
}
