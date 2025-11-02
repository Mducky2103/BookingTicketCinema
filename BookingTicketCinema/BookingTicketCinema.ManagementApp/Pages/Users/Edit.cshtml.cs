using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingTicketCinema.ManagementApp.Pages.Users
{
    [Authorize(Policy = "RequireAdmin")]
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public EditModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty]
        public EditUserViewModel User { get; set; } = new(); // Dùng cho Form
        public SelectList RoleList { get; set; }
        public string? ErrorMessage { get; set; }

        // Hàm helper lấy client đã xác thực
        private async Task<HttpClient> GetAuthorizedClientAsync()
        {
            var accessToken = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            var httpClient = _httpClientFactory.CreateClient("ApiClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return httpClient;
        }

        // Hàm helper tải Dropdown Role
        private void LoadRoleList()
        {
            var roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "Staff", Text = "Staff" },
                new SelectListItem { Value = "Customer", Text = "Customer" }
            };
            RoleList = new SelectList(roles, "Value", "Text", User.Role);
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

                var userFromApi = await response.Content.ReadFromJsonAsync<UserListViewModel>();

                if (userFromApi == null)
                {
                    ErrorMessage = "Không đọc được dữ liệu người dùng.";
                    return Page();
                }

                // Gán dữ liệu từ DTO (UserListViewModel)
                //    sang ViewModel của Form (EditUserViewModel)
                User.Id = userFromApi.Id;
                User.Email = userFromApi.Email;
                User.FullName = userFromApi.FullName;
                User.PhoneNumber = userFromApi.PhoneNumber;
                User.Role = userFromApi.Role; 

                LoadRoleList(); 
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            LoadRoleList();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var httpClient = await GetAuthorizedClientAsync();
                var apiBaseUrl = _configuration["ApiBaseUrl"];

                var updateUserDto = new
                {
                    Email = User.Email,
                    FullName = User.FullName,
                    PhoneNumber = User.PhoneNumber
                };
                var jsonContent = new StringContent(JsonSerializer.Serialize(updateUserDto), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{apiBaseUrl}/api/User/{User.Id}", jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Lỗi khi cập nhật thông tin user.");
                    return Page();
                }

                var roleContent = new StringContent(JsonSerializer.Serialize(User.Role), Encoding.UTF8, "application/json");
                var roleResponse = await httpClient.PutAsync($"{apiBaseUrl}/api/User/{User.Id}/roles", roleContent);

                if (!roleResponse.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Lỗi khi cập nhật Role.");
                    return Page();
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
