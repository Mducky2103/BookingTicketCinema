using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BookingTicketCinema.WebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.Client.Pages.Profile
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public EditProfileVm Input { get; set; } = new();

        public string? Message { get; set; }

        public async Task OnGet()
        {
            var http = _httpClientFactory.CreateClient("ApiClient");

            string? token = User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;

            if (token == null)
                return;

            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await http.GetAsync("api/UserProfile");
            var json = await response.Content.ReadAsStringAsync();

            var profile = JsonSerializer.Deserialize<UserProfileVm>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (profile != null)
            {
                Input.FullName = profile.FullName;
                Input.Gender = profile.Gender;
                Input.DateOfBirth = profile.DateOfBirth;
            }
        }

        public async Task<IActionResult> OnPost()
        {
            var http = _httpClientFactory.CreateClient("ApiClient");

            string? token = User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(Input),
                Encoding.UTF8,
                "application/json");

            var response = await http.PutAsync("api/EditUserProfile", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                Message = "Cập nhật thành công!";
            }
            else
            {
                Message = "Có lỗi xảy ra!";
            }

            return Page();
        }
    }
}
