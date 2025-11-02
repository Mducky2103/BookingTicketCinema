using System.Net.Http.Headers;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingTicketCinema.ManagementApp.Pages.Users
{
    [Authorize(Policy = "RequireAdmin")] // Policy từ Program.cs
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterRole { get; set; }
        public SelectList RoleList { get; set; } = new(new[] { "Admin", "Staff", "Customer" });
        public IndexModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalUsers { get; set; }
        public List<UserListViewModel> PaginatedUsers { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Thiết lập RoleList cho dropdown
            var roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Tất cả vai trò" },
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "Staff", Text = "Staff" },
                new SelectListItem { Value = "Customer", Text = "Customer" }
            };
            RoleList = new SelectList(roles, "Value", "Text", FilterRole);

            // Lấy token
            var accessToken = User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            if (string.IsNullOrEmpty(accessToken))
            {
                ErrorMessage = "Lỗi xác thực.";
                return Page();
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiClient");
                var apiBaseUrl = _configuration["ApiBaseUrl"];
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // 1. GỌI API (LẤY TẤT CẢ USER)
                var response = await httpClient.GetAsync($"{apiBaseUrl}/api/User");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = $"Không thể tải danh sách. Lỗi: {response.StatusCode}";
                    return Page();
                }

                var allUsers = await response.Content.ReadFromJsonAsync<List<UserListViewModel>>() ?? new();

                // LỌC (CLIENT-SIDE)
                var filteredUsers = allUsers.AsQueryable();

                // Lọc theo Tên/Email (không phân biệt hoa thường)
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    filteredUsers = filteredUsers.Where(u =>
                        (u.FullName != null && u.FullName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (u.Email != null && u.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                    );
                }

                // Lọc theo Role
                if (!string.IsNullOrEmpty(FilterRole))
                {
                    filteredUsers = filteredUsers.Where(u => u.Role == FilterRole);
                }

                var finalUserList = filteredUsers.ToList();
                TotalUsers = finalUserList.Count;

                // PHÂN TRANG (CLIENT-SIDE)
                TotalPages = (int)Math.Ceiling(TotalUsers / (double)PageSize);
                if (CurrentPage < 1) CurrentPage = 1;
                if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;

                PaginatedUsers = finalUserList
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Đã xảy ra lỗi: {ex.Message}";
            }

            return Page();
        }
    }
}
