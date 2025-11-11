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

        // 🧠 Gắn JWT token từ cookie claims hoặc session vào header Authorization
        private void AttachToken()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return;

            // 🔹 Lấy token từ Claims (cookie đăng nhập)
            var token = context.User?.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;

            // 🔹 Nếu không có trong claim, thử lấy từ Session
            if (string.IsNullOrEmpty(token))
                token = context.Session.GetString("AccessToken");

            // 🔹 Gắn token vào header
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                _logger.LogWarning("⚠️ Không tìm thấy token đăng nhập để gắn vào request.");
            }
        }


        // -------------------------
        // GỌI API CHUẨN HÓA
        // -------------------------
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
    }
}
