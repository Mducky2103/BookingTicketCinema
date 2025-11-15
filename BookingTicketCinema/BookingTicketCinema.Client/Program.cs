using System.Net.Http.Headers;
using BookingTicketCinema.WebApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// Cấu hình HttpClientFactory để gọi Backend API
builder.Services.AddHttpClient("ApiClient", (serviceProvider, client) =>
{
    // Lấy URL của Backend API từ appsettings.json
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
    client.BaseAddress = new Uri(apiBaseUrl!);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
// Cần thiết để truy cập HttpContext (và token) từ các dịch vụ khác
builder.Services.AddHttpContextAccessor();

// Cấu hình Authentication cho Client (QUAN TRỌNG)
// Client này sử dụng Cookie để quản lý phiên đăng nhập của *chính nó*.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Chuyển hướng đến trang này nếu chưa đăng nhập
        options.LogoutPath = "/Auth/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(1); // Thời gian cookie tồn tại
        options.AccessDeniedPath = "/AccessDenied";
    });

// Thêm dịch vụ Session (tùy chọn, nhưng hữu ích)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CustomerOnly", policy =>
        policy.RequireRole("Customer"));
});

// Cấu hình bảo vệ thư mục
builder.Services.Configure<RazorPagesOptions>(options =>
{
    // Chỉ bảo vệ các trang Profile và Booking
    // Trang chủ (Index), trang Phim (Movie) sẽ công khai
    options.Conventions.AuthorizeFolder("/Profile");
    options.Conventions.AuthorizeFolder("/Booking");
});

builder.Services.AddScoped<IApiClientService, ApiClientService>();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Kích hoạt session

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
