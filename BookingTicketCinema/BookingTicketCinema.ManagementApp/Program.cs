using System.Net.Http.Headers;
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

// Cookie này chỉ dành cho trang quản lý
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.AccessDeniedPath = "/AccessDenied";
    });

// Định nghĩa Policy dựa trên Role
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("RequireStaff", policy =>
        policy.RequireRole("Staff", "Admin")); // Staff hoặc Admin đều được
});
// Gán Policy cho thư mục
builder.Services.Configure<RazorPagesOptions>(options =>
{
    // Tất cả các trang (trừ Auth) đều yêu cầu ít nhất là Staff
    options.Conventions.AuthorizeFolder("/", "RequireStaff");

    options.Conventions.AllowAnonymousToPage("/Auth/Login");
    options.Conventions.AllowAnonymousToPage("/Auth/Logout");
    // Các trang này yêu cầu quyền Admin
    options.Conventions.AuthorizeFolder("/Users", "RequireAdmin");
    //options.Conventions.AuthorizeFolder("/PriceRules", "RequireAdmin");
    //options.Conventions.AuthorizeFolder("/Promotions", "RequireAdmin");
    //options.Conventions.AuthorizeFolder("/Rooms", "RequireAdmin");
    //options.Conventions.AuthorizeFolder("/Reports", "RequireAdmin");

});
// Thêm dịch vụ Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
