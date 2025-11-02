using System.Net.Http.Headers;
using BookingTicketCinema.ManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace BookingTicketCinema.ManagementApp.Pages.SeatGroup
{
    [Authorize(Policy = "RequireAdmin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IndexModel> _logger;

        public List<SeatGroupViewModel> SeatGroups { get; set; } = new();
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

                var seatGroupsTask = httpClient.GetAsync($"{apiBaseUrl}/api/seatgroups");
                var roomsTask = httpClient.GetAsync($"{apiBaseUrl}/api/rooms");

                await Task.WhenAll(seatGroupsTask, roomsTask);

                // Handle seat groups response
                if (seatGroupsTask.Result.IsSuccessStatusCode)
                {
                    var seatGroupsResponse = await seatGroupsTask.Result.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(seatGroupsResponse))
                    {
                        SeatGroups = JsonSerializer.Deserialize<List<SeatGroupViewModel>>(seatGroupsResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }

                // Handle rooms response
                if (roomsTask.Result.IsSuccessStatusCode)
                {
                    var roomsResponse = await roomsTask.Result.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(roomsResponse))
                    {
                        Rooms = JsonSerializer.Deserialize<List<RoomViewModel>>(roomsResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }

                // Map room names to seat groups
                var roomDict = Rooms.ToDictionary(r => r.RoomId, r => r.Name);
                foreach (var seatGroup in SeatGroups)
                {
                    if (roomDict.ContainsKey(seatGroup.RoomId))
                    {
                        seatGroup.RoomName = roomDict[seatGroup.RoomId];
                    }
                }

                var tempSuccess = TempData["SuccessMessage"]?.ToString();
                if (!string.IsNullOrEmpty(tempSuccess)) SuccessMessage = tempSuccess;

                var tempError = TempData["ErrorMessage"]?.ToString();
                if (!string.IsNullOrEmpty(tempError)) ErrorMessage = tempError;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading seat groups");
                ErrorMessage = "Không thể tải danh sách nhóm ghế. Vui lòng thử lại sau.";
            }
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] string groupName, [FromForm] int type, [FromForm] int roomId)
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

                var request = new CreateSeatGroupRequest { GroupName = groupName, Type = type, RoomId = roomId };
                var jsonContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
                
                var response = await httpClient.PostAsync($"{apiBaseUrl}/api/seatgroups", jsonContent);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Tạo nhóm ghế thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tạo nhóm ghế.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating seat group");
                TempData["ErrorMessage"] = $"Không thể tạo nhóm ghế: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync([FromForm] int id, [FromForm] string? groupName, [FromForm] int? type, [FromForm] int? roomId)
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

                var request = new UpdateSeatGroupRequest { GroupName = groupName, Type = type, RoomId = roomId };
                var jsonContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
                
                var response = await httpClient.PutAsync($"{apiBaseUrl}/api/seatgroups/{id}", jsonContent);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật nhóm ghế thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật nhóm ghế.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating seat group");
                TempData["ErrorMessage"] = $"Không thể cập nhật nhóm ghế: {ex.Message}";
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

                var response = await httpClient.DeleteAsync($"{apiBaseUrl}/api/seatgroups/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Xóa nhóm ghế thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa nhóm ghế.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting seat group");
                TempData["ErrorMessage"] = $"Không thể xóa nhóm ghế: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}

