using System.Net.Http.Headers;
using System.Text.Json;
using BookingTicketCinema.WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.Client.Pages.Profile
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public UserProfileVm? Profile { get; set; }

        public async Task OnGet()
        {
            var http = _httpClientFactory.CreateClient("ApiClient");

            // Lấy JWT từ claims trong cookie
            string? token = User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;

            if (token == null)
                return;

            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await http.GetAsync("api/UserProfile");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Profile = JsonSerializer.Deserialize<UserProfileVm>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
        }
    }
}
