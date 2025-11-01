using System.Net.Http.Headers;
using BookingTicketCinema.ManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace BookingTicketCinema.ManagementApp.Pages.SeatLayout
{
    [Authorize(Policy = "RequireAdmin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IndexModel> _logger;

        public List<SeatViewModel> Seats { get; set; } = new();
        public List<RoomViewModel> Rooms { get; set; } = new();
        public List<SeatGroupViewModel> SeatGroups { get; set; } = new();

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

                var seatsTask = httpClient.GetAsync($"{apiBaseUrl}/api/seats");
                var roomsTask = httpClient.GetAsync($"{apiBaseUrl}/api/rooms");
                var seatGroupsTask = httpClient.GetAsync($"{apiBaseUrl}/api/seatgroups");

                await Task.WhenAll(seatsTask, roomsTask, seatGroupsTask);

                // Handle seats response
                if (seatsTask.Result.IsSuccessStatusCode)
                {
                    var seatsContent = await seatsTask.Result.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(seatsContent))
                    {
                        Seats = JsonSerializer.Deserialize<List<SeatViewModel>>(seatsContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }

                // Handle rooms response
                if (roomsTask.Result.IsSuccessStatusCode)
                {
                    var roomsContent = await roomsTask.Result.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(roomsContent))
                    {
                        Rooms = JsonSerializer.Deserialize<List<RoomViewModel>>(roomsContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }

                // Handle seat groups response
                if (seatGroupsTask.Result.IsSuccessStatusCode)
                {
                    var seatGroupsContent = await seatGroupsTask.Result.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(seatGroupsContent))
                    {
                        SeatGroups = JsonSerializer.Deserialize<List<SeatGroupViewModel>>(seatGroupsContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }

                // Map room and seat group names
                var roomDict = Rooms.ToDictionary(r => r.RoomId, r => r.Name);
                var seatGroupDict = SeatGroups.ToDictionary(sg => sg.SeatGroupId, sg => sg.GroupName);
                
                foreach (var seat in Seats)
                {
                    if (roomDict.ContainsKey(seat.RoomId))
                    {
                        seat.RoomName = roomDict[seat.RoomId];
                    }
                    if (seatGroupDict.ContainsKey(seat.SeatGroupId))
                    {
                        seat.SeatGroupName = seatGroupDict[seat.SeatGroupId];
                    }
                }

                var tempSuccess = TempData["SuccessMessage"]?.ToString();
                if (!string.IsNullOrEmpty(tempSuccess)) SuccessMessage = tempSuccess;

                var tempError = TempData["ErrorMessage"]?.ToString();
                if (!string.IsNullOrEmpty(tempError)) ErrorMessage = tempError;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading seats");
                ErrorMessage = "Không thể tải danh sách ghế. Vui lòng thử lại sau.";
            }
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] string seatNumber, [FromForm] int status, [FromForm] int roomId, [FromForm] int seatGroupId)
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

                var request = new CreateSeatRequest { SeatNumber = seatNumber, Status = status, RoomId = roomId, SeatGroupId = seatGroupId };
                var jsonContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
                
                var response = await httpClient.PostAsync($"{apiBaseUrl}/api/seats", jsonContent);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Tạo ghế thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tạo ghế.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating seat");
                TempData["ErrorMessage"] = $"Không thể tạo ghế: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync([FromForm] int id, [FromForm] string? seatNumber, [FromForm] int? status, [FromForm] int? seatGroupId)
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

                var request = new UpdateSeatRequest { SeatNumber = seatNumber, Status = status, SeatGroupId = seatGroupId };
                var jsonContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
                
                var response = await httpClient.PutAsync($"{apiBaseUrl}/api/seats/{id}", jsonContent);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật ghế thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật ghế.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating seat");
                TempData["ErrorMessage"] = $"Không thể cập nhật ghế: {ex.Message}";
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

                var response = await httpClient.DeleteAsync($"{apiBaseUrl}/api/seats/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Xóa ghế thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa ghế.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting seat");
                TempData["ErrorMessage"] = $"Không thể xóa ghế: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetGetSeatGroupsByRoomAsync(int roomId)
        {
            try
            {
                var accessToken = User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
                if (string.IsNullOrEmpty(accessToken))
                {
                    return new JsonResult(new List<SeatGroupViewModel>());
                }

                var httpClient = _httpClientFactory.CreateClient("ApiClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var apiBaseUrl = _configuration["ApiBaseUrl"];

                var response = await httpClient.GetAsync($"{apiBaseUrl}/api/seatgroups/room/{roomId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        var seatGroups = JsonSerializer.Deserialize<List<SeatGroupViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<SeatGroupViewModel>();
                        return new JsonResult(seatGroups);
                    }
                }
                
                return new JsonResult(new List<SeatGroupViewModel>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading seat groups for room {RoomId}", roomId);
                return new JsonResult(new List<SeatGroupViewModel>());
            }
        }
    }
}

