using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages.Auth
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public LoginModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public void OnGet(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                Response.Redirect("/");
            }
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl ??= Url.Content("~/"); // Mặc định về trang chủ (Dashboard)

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiClient");
                var apiBaseUrl = _configuration["ApiBaseUrl"];

                // 1. GỌI API LOGIN
                var jsonContent = new StringContent(JsonSerializer.Serialize(Input), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{apiBaseUrl}/api/signin", jsonContent); // Dùng endpoint /api/signin của bạn

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác.");
                    return Page();
                }

                // 2. NHẬN VÀ GIẢI MÃ TOKEN
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponseModel>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (string.IsNullOrEmpty(loginResponse?.Token))
                {
                    ModelState.AddModelError(string.Empty, "Lỗi: API không trả về token.");
                    return Page();
                }

                var tokenString = loginResponse.Token;
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(tokenString);

                string roleClaimType = "role"; // HOẶC "Role", hoặc bất cứ thứ gì bạn thấy

                // 3. LẤY CLAIMS VÀ KIỂM TRA ROLE
                var claims = jwtToken.Claims.ToList();
                var roleClaims = claims.Where(c => c.Type == roleClaimType).Select(c => c.Value).ToList();

                if (!roleClaims.Contains("Admin") && !roleClaims.Contains("Staff"))
                {
                    ModelState.AddModelError(string.Empty, "Bạn không có quyền truy cập trang quản lý.");
                    return Page();
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);

                var profileResponse = await httpClient.GetAsync($"{apiBaseUrl}/api/UserProfile");
                string fullName = Input.Email; 

                if (profileResponse.IsSuccessStatusCode)
                {
                    var profileJson = await profileResponse.Content.ReadAsStringAsync();
                    var userProfile = JsonSerializer.Deserialize<UserProfileModel>(profileJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (!string.IsNullOrEmpty(userProfile?.FullName))
                    {
                        fullName = userProfile.FullName;
                    }
                }

                // 4. LƯU THÔNG TIN VÀO COOKIE
                claims.Add(new Claim(ClaimTypes.Name, fullName));

                // LƯU TOKEN VÀO COOKIE: Để các API call sau có thể dùng
                claims.Add(new Claim("access_token", tokenString));

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name, // Dùng claim "Name" cho User.Identity.Name
                    roleClaimType
                );

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true, // Ghi nhớ đăng nhập
                        ExpiresUtc = jwtToken.ValidTo // Cookie hết hạn cùng JWT
                    });

                // 5. Chuyển hướng về trang chủ (Index)
                return LocalRedirect(ReturnUrl);
            }
            catch (Exception ex)
            {
                // Ghi log ex
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi. Vui lòng thử lại.");
                return Page();
            }
        }
    }
}
