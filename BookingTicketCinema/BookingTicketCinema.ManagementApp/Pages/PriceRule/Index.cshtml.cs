using System.Net.Http.Headers;
using BookingTicketCinema.ManagementApp.Models;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace BookingTicketCinema.ManagementApp.Pages.PriceRule
{
    [Authorize(Policy = "RequireAdmin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IndexModel> _logger;

        public List<PriceRuleViewModel> PriceRules { get; set; } = new();
        public List<SeatGroupViewModel> SeatGroups { get; set; } = new();
        public List<ShowtimeViewModel> Showtimes { get; set; } = new();

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

                var priceRulesTask = httpClient.GetAsync($"{apiBaseUrl}/api/pricerules");
                var seatGroupsTask = httpClient.GetAsync($"{apiBaseUrl}/api/seatgroups");
                var showtimesTask = httpClient.GetAsync($"{apiBaseUrl}/api/showtimes");

                await Task.WhenAll(priceRulesTask, seatGroupsTask, showtimesTask);

                // Handle price rules response
                if (priceRulesTask.Result.IsSuccessStatusCode)
                {
                    var priceRulesContent = await priceRulesTask.Result.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(priceRulesContent))
                    {
                        PriceRules = JsonSerializer.Deserialize<List<PriceRuleViewModel>>(priceRulesContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
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

                // Handle showtimes response (optional - may not exist yet)
                if (showtimesTask.Result.IsSuccessStatusCode)
                {
                    var showtimesContent = await showtimesTask.Result.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(showtimesContent))
                    {
                        Showtimes = JsonSerializer.Deserialize<List<ShowtimeViewModel>>(showtimesContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }

                // Map seat group names and showtime names to price rules
                var seatGroupDict = SeatGroups.ToDictionary(sg => sg.SeatGroupId, sg => sg.GroupName);
                var showtimeDict = Showtimes.ToDictionary(st => st.ShowtimeId, st => $"{st.MovieName} ({st.RoomName}) - {st.StartTime:dd/MM/yyyy HH:mm}");
                
                foreach (var priceRule in PriceRules)
                {
                    if (seatGroupDict.ContainsKey(priceRule.SeatGroupId))
                    {
                        priceRule.SeatGroupName = seatGroupDict[priceRule.SeatGroupId];
                    }
                    
                    if (priceRule.ShowtimeId.HasValue && showtimeDict.ContainsKey(priceRule.ShowtimeId.Value))
                    {
                        priceRule.ShowtimeName = showtimeDict[priceRule.ShowtimeId.Value];
                    }
                }

                var tempSuccess = TempData["SuccessMessage"]?.ToString();
                if (!string.IsNullOrEmpty(tempSuccess)) SuccessMessage = tempSuccess;

                var tempError = TempData["ErrorMessage"]?.ToString();
                if (!string.IsNullOrEmpty(tempError)) ErrorMessage = tempError;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading price rules");
                ErrorMessage = "Không thể tải danh sách quy tắc giá. Vui lòng thử lại sau.";
            }
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] decimal basePrice, [FromForm] int dayOfWeek, [FromForm] int slot, [FromForm] int seatGroupId, [FromForm] int? showtimeId)
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

                var request = new CreatePriceRuleRequest 
                { 
                    BasePrice = basePrice, 
                    DayOfWeek = dayOfWeek, 
                    Slot = slot, 
                    SeatGroupId = seatGroupId, 
                    ShowtimeId = showtimeId == 0 ? null : showtimeId 
                };
                var jsonContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
                
                var response = await httpClient.PostAsync($"{apiBaseUrl}/api/pricerules", jsonContent);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Tạo quy tắc giá thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tạo quy tắc giá.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating price rule");
                TempData["ErrorMessage"] = $"Không thể tạo quy tắc giá: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync([FromForm] int id, [FromForm] decimal? basePrice, [FromForm] int? dayOfWeek, [FromForm] int? slot, [FromForm] int? seatGroupId, [FromForm] int? showtimeId)
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

                var request = new UpdatePriceRuleRequest 
                { 
                    BasePrice = basePrice, 
                    DayOfWeek = dayOfWeek, 
                    Slot = slot, 
                    SeatGroupId = seatGroupId, 
                    ShowtimeId = (showtimeId == 0) ? null : showtimeId  // Keep -1 to be handled by API service
                };
                var jsonContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
                
                var response = await httpClient.PutAsync($"{apiBaseUrl}/api/pricerules/{id}", jsonContent);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật quy tắc giá thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật quy tắc giá.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating price rule");
                TempData["ErrorMessage"] = $"Không thể cập nhật quy tắc giá: {ex.Message}";
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

                var response = await httpClient.DeleteAsync($"{apiBaseUrl}/api/pricerules/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Xóa quy tắc giá thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa quy tắc giá.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting price rule");
                TempData["ErrorMessage"] = $"Không thể xóa quy tắc giá: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetShowtimesByRoomAsync(int roomId)
        {
            try
            {
                var accessToken = User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
                if (string.IsNullOrEmpty(accessToken))
                {
                    return new JsonResult(new List<ShowtimeViewModel>());
                }

                var httpClient = _httpClientFactory.CreateClient("ApiClient");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var apiBaseUrl = _configuration["ApiBaseUrl"];

                var response = await httpClient.GetAsync($"{apiBaseUrl}/api/showtimes/room/{roomId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        var showtimes = JsonSerializer.Deserialize<List<ShowtimeViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ShowtimeViewModel>();
                        return new JsonResult(showtimes);
                    }
                }
                
                return new JsonResult(new List<ShowtimeViewModel>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading showtimes for room {RoomId}", roomId);
                return new JsonResult(new List<ShowtimeViewModel>());
            }
        }
    }
}

