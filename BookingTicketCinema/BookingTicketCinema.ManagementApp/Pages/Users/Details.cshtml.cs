using System.Net.Http.Headers;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages.Users
{
    [Authorize(Policy = "RequireAdmin")]
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DetailsModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public UserListViewModel? User { get; set; } // Dùng lại ViewModel
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ErrorMessage = "ID người dùng không hợp lệ.";
                return Page();
            }

            var accessToken = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            var httpClient = _httpClientFactory.CreateClient("ApiClient");
            var apiBaseUrl = _configuration["ApiBaseUrl"];
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                var response = await httpClient.GetAsync($"{apiBaseUrl}/api/User/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = $"Không tìm thấy người dùng (Lỗi: {response.StatusCode}).";
                    return Page();
                }

                User = await response.Content.ReadFromJsonAsync<UserListViewModel>();


                var roleResponse = await httpClient.GetAsync($"{apiBaseUrl}/api/User/{id}/roles");
                if (roleResponse.IsSuccessStatusCode && User != null)
                {
                    var roles = await roleResponse.Content.ReadFromJsonAsync<List<string>>();
                    User.Role = roles?.FirstOrDefault() ?? "N/A";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return Page();
        }
    }
}
