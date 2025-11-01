using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using BookingTicketCinema.Client.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.Client.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl ??= Url.Content("~/"); // Trang chủ nếu không có returnUrl

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var httpClient = _httpClientFactory.CreateClient("ApiClient");

            // 1. Chuẩn bị dữ liệu gửi đến API
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(Input),
                Encoding.UTF8,
                "application/json");

            // 2. Gọi API /api/signin
            var response = await httpClient.PostAsync("api/signin", jsonContent);

            // 3. Xử lý kết quả
            if (response.IsSuccessStatusCode)
            {
                // Đăng nhập API thành công, nhận JWT
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponseModel>(jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (loginResponse?.Token == null)
                {
                    ModelState.AddModelError(string.Empty, "Không nhận được token từ API.");
                    return Page();
                }

                // 4. Giải mã JWT để lấy Claims mà Backend đã tạo
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(loginResponse.Token);
                var claims = jwtToken.Claims.ToList();

                // Thêm chính token vào Claims để sử dụng sau này
                claims.Add(new Claim("access_token", loginResponse.Token));

                // 5. Tạo phiên đăng nhập (Cookie) cho Client Razor Pages
                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    "name", // Sử dụng claim "name" (nếu có) làm User.Identity.Name
                    ClaimTypes.Role // Sử dụng claim "role" làm User.IsInRole()
                );

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true, // Tùy chọn: ghi nhớ đăng nhập
                        ExpiresUtc = jwtToken.ValidTo // Cookie hết hạn cùng lúc với JWT
                    });

                // 6. Chuyển hướng đến trang được yêu cầu
                return LocalRedirect(ReturnUrl);
            }
            else
            {
                // Đăng nhập API thất bại
                var errorContent = await response.Content.ReadAsStringAsync();
                // Cố gắng đọc lỗi từ API
                try
                {
                    var errorResponse = JsonDocument.Parse(errorContent);
                    var message = errorResponse.RootElement.GetProperty("message").GetString();
                    ModelState.AddModelError(string.Empty, message ?? "Đăng nhập thất bại.");
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Đăng nhập thất bại. Vui lòng thử lại.");
                }

                return Page();
            }
        }
    }
}
