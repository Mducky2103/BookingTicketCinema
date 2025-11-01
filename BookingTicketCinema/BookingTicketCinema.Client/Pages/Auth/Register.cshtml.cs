using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BookingTicketCinema.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.Client.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public RegisterModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty]
        public RegisterViewModel Input { get; set; } = new();
        public string MaxDate { get; } = DateTime.Today.ToString("yyyy-MM-dd");
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Khớp với 'UserRegistrationModel' của backend
            var apiPayload = new
            {
                FullName = Input.FullName,
                Email = Input.Email,
                Gender = Input.Gender,
                DOB = Input.DOB, 
                Password = Input.Password,
                Role = "Customer" 
            };

            var httpClient = _httpClientFactory.CreateClient("ApiClient");
            var apiBaseUrl = _configuration["ApiBaseUrl"];

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(apiPayload),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync($"{apiBaseUrl}/api/signup", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["RegisterSuccess"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToPage("/Auth/Login");
            }

            // Xử lý lỗi (Backend của bạn trả về 'errors')
            var errorContent = await response.Content.ReadAsStringAsync();
            try
            {
                var errorDoc = JsonDocument.Parse(errorContent);
                // Tìm 'errors' (từ code CreateUser của bạn)
                if (errorDoc.RootElement.TryGetProperty("errors", out var errorsElement))
                {
                    foreach (var error in errorsElement.EnumerateArray())
                    {
                        ModelState.AddModelError(string.Empty, error.GetString() ?? "Lỗi đăng ký");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi không xác định.");
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi không xác định.");
                Console.WriteLine($"API Error Content: {errorContent}"); // Ghi log lỗi
            }

            return Page();
        }
    }
}
