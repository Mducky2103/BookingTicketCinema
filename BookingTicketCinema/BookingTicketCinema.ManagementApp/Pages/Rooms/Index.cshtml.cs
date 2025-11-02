using System.Net.Http.Headers;
using BookingTicketCinema.ManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace BookingTicketCinema.ManagementApp.Pages.Rooms
{
    [Authorize(Policy = "RequireAdmin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IndexModel> _logger;

        public List<RoomViewModel> Rooms { get; set; } = new();

        [BindProperty]
        public string? SuccessMessage { get; set; }

        [BindProperty]
        public string? ErrorMessage { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<IndexModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            try
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

                var response = await httpClient.GetAsync($"{apiBaseUrl}/api/rooms");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        Rooms = JsonSerializer.Deserialize<List<RoomViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }

                var tempSuccess = TempData["SuccessMessage"]?.ToString();
                if (!string.IsNullOrEmpty(tempSuccess)) SuccessMessage = tempSuccess;

                var tempError = TempData["ErrorMessage"]?.ToString();
                if (!string.IsNullOrEmpty(tempError)) ErrorMessage = tempError;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading rooms");
                ErrorMessage = "Không thể tải danh sách phòng. Vui lòng thử lại sau.";
            }
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] string name, [FromForm] int capacity, [FromForm] int type)
        {
            try
            {
                var accessToken = User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
                if (string.IsNullOrEmpty(accessToken))
                {
                    TempData["ErrorMessage"] = "Lỗi xác thực. Vui lòng đăng nhập lại.";
                    return RedirectToPage();
                }

                var httpClient = _httpClientFactory.CreateClient("ApiClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var apiBaseUrl = _configuration["ApiBaseUrl"];

                var request = new CreateRoomRequest { Name = name, Capacity = capacity, Type = type };
                var jsonContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
                
                var response = await httpClient.PostAsync($"{apiBaseUrl}/api/rooms", jsonContent);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Tạo phòng thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tạo phòng.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room");
                TempData["ErrorMessage"] = $"Không thể tạo phòng: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync([FromForm] int id, [FromForm] string? name, [FromForm] int? capacity, [FromForm] int? type)
        {
            try
            {
                var accessToken = User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
                if (string.IsNullOrEmpty(accessToken))
                {
                    TempData["ErrorMessage"] = "Lỗi xác thực. Vui lòng đăng nhập lại.";
                    return RedirectToPage();
                }

                var httpClient = _httpClientFactory.CreateClient("ApiClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var apiBaseUrl = _configuration["ApiBaseUrl"];

                var request = new UpdateRoomRequest { Name = name, Capacity = capacity, Type = type };
                var jsonContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
                
                var response = await httpClient.PutAsync($"{apiBaseUrl}/api/rooms/{id}", jsonContent);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật phòng thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật phòng.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating room");
                TempData["ErrorMessage"] = $"Không thể cập nhật phòng: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromForm] int id)
        {
            try
            {
                var accessToken = User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
                if (string.IsNullOrEmpty(accessToken))
                {
                    TempData["ErrorMessage"] = "Lỗi xác thực. Vui lòng đăng nhập lại.";
                    return RedirectToPage();
                }

                var httpClient = _httpClientFactory.CreateClient("ApiClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var apiBaseUrl = _configuration["ApiBaseUrl"];

                var response = await httpClient.DeleteAsync($"{apiBaseUrl}/api/rooms/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Xóa phòng thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa phòng.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting room");
                TempData["ErrorMessage"] = $"Không thể xóa phòng: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}

