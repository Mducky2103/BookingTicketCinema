using System.Net.Http.Headers;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages.Users
{
    [Authorize(Policy = "RequireAdmin")]
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DeleteModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty]
        public UserListViewModel User { get; set; } = new(); // Dùng lại ViewModel
        public string? ErrorMessage { get; set; }

        private async Task<HttpClient> GetAuthorizedClientAsync()
        {
            var accessToken = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            var httpClient = _httpClientFactory.CreateClient("ApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return httpClient;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ErrorMessage = "ID người dùng không hợp lệ.";
                return Page();
            }

            try
            {
                var httpClient = await GetAuthorizedClientAsync();
                var apiBaseUrl = _configuration["ApiBaseUrl"];

                var response = await httpClient.GetAsync($"{apiBaseUrl}/api/User/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Không tìm thấy người dùng.";
                    return Page();
                }

                User = await response.Content.ReadFromJsonAsync<UserListViewModel>() ?? new UserListViewModel();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(User.Id))
            {
                ErrorMessage = "ID người dùng không hợp lệ.";
                return Page();
            }

            try
            {
                var httpClient = await GetAuthorizedClientAsync();
                var apiBaseUrl = _configuration["ApiBaseUrl"];

                // Gọi API DELETE
                var response = await httpClient.DeleteAsync($"{apiBaseUrl}/api/User/{User.Id}");

                if (response.IsSuccessStatusCode)
                {
                    // Xóa thành công, quay về trang Index
                    return RedirectToPage("./Index");
                }

                ErrorMessage = $"Lỗi khi xóa người dùng. (Lỗi: {response.StatusCode})";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
