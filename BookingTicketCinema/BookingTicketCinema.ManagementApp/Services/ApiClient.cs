using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace BookingTicketCinema.ManagementApp.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ApiClient> _logger;

        public ApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<ApiClient> logger)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        //Gắn JWT token từ cookie claims hoặc session vào header Authorization
        private void AttachToken()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return;

            var token = context.User?.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;

            if (string.IsNullOrEmpty(token))
                token = context.Session.GetString("AccessToken");

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                _logger.LogWarning(" Không tìm thấy token đăng nhập để gắn vào request.");
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            AttachToken();
            return await _httpClient.GetAsync(endpoint);
        }

        public async Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent? content = null)
        {
            AttachToken();
            return await _httpClient.PostAsync(endpoint, content);
        }

        public async Task<HttpResponseMessage> PutAsync(string endpoint, HttpContent? content = null)
        {
            AttachToken();
            return await _httpClient.PutAsync(endpoint, content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            AttachToken();
            var res = await _httpClient.DeleteAsync(endpoint);
            _logger.LogInformation($"DELETE {endpoint} => {res.StatusCode}");
            return res;
        }


        // POS Bước 1: Chọn Phim
        public async Task<HttpResponseMessage> GetMoviesAsync()
        {
            // (API này bên Backend đang AllowAnonymous, 
            // nhưng chúng ta cứ gắn token để bảo mật nếu cần)
            return await GetAsync("api/Movie");
        }

        // POS Bước 2: Chọn Suất chiếu
        public async Task<HttpResponseMessage> GetShowtimesByMovieAsync(int movieId)
        {
            return await GetAsync($"api/showtimes/GetShowtimesByMovie/{movieId}");
        }

        // POS Bước 3: Lấy chi tiết sơ đồ ghế
        public async Task<HttpResponseMessage> GetShowtimeForBookingAsync(int showtimeId)
        {
            return await GetAsync($"api/showtimes/GetShowtimeById/{showtimeId}");
        }

        public async Task<HttpResponseMessage> GetRoomByIdAsync(int roomId)
        {
            return await GetAsync($"api/rooms/{roomId}");
        }

        public async Task<HttpResponseMessage> GetSeatsByRoomAsync(int roomId)
        {
            return await GetAsync($"api/seats/room/{roomId}");
        }

        public async Task<HttpResponseMessage> GetSeatGroupsByRoomAsync(int roomId)
        {
            return await GetAsync($"api/seatgroups/room/{roomId}");
        }

        public async Task<HttpResponseMessage> GetTakenSeatIdsAsync(int showtimeId)
        {
            // API này trong TicketController
            return await GetAsync($"api/ticket/showtime/{showtimeId}/taken-seats");
        }

        // POS Bước 4: Bán vé
        public async Task<HttpResponseMessage> BookAtCounterAsync(HttpContent content)
        {
            return await PostAsync("api/POS/book", content);
        }

        // POS Bước 5: Lấy Hóa đơn
        public async Task<HttpResponseMessage> GetReceiptAsync(int paymentId)
        {
            return await GetAsync($"api/pos/receipt/{paymentId}");
        }
        public async Task<HttpResponseMessage> GetMyPOSHistoryAsync(int pageNumber, int pageSize)
        {
            return await GetAsync($"api/pos/my-history?pageNumber={pageNumber}&pageSize={pageSize}");
        }
        public async Task<HttpResponseMessage> GetDashboardStatsAsync()
        {
            return await GetAsync("api/statistics/dashboard");
        }
        public async Task<HttpResponseMessage> GetSalesReportAsync(DateTime startDate, DateTime endDate)
        {
            // Format ngày theo chuẩn yyyy-MM-dd để API hiểu
            var start = startDate.ToString("yyyy-MM-dd");
            var end = endDate.ToString("yyyy-MM-dd");

            return await GetAsync($"api/statistics/sales-report?startDate={start}&endDate={end}");
        }
        public async Task<HttpResponseMessage> CreatePromotionAsync(HttpContent content)
        {
            return await PostAsync("api/Promotion", content);
        }
        public async Task<HttpResponseMessage> GetPromotionsAsync()
        {
            return await GetAsync("api/Promotion");
        }

        public async Task<HttpResponseMessage> GetPromotionByIdAsync(int id)
        {
            return await GetAsync($"api/Promotion/{id}");
        }

        public async Task<HttpResponseMessage> UpdatePromotionAsync(int id, HttpContent content)
        {
            return await PutAsync($"api/Promotion/{id}", content);
        }
    }
}
