using Microsoft.AspNetCore.Authorization;

namespace BookingCinemaTicket.Controllers
{
    public static class AuthorizationDemoEndpoints
    {
        public static IEndpointRouteBuilder MapAuthorizationDemoEndpoints(this IEndpointRouteBuilder app)
        {
            //Chỉ dành cho admin quản lý hệ thống
            app.MapGet("/AdminOnly", AdminOnly);

            //Dành cho admin và manager (quản lý rạp, lịch chiếu, phim)
            app.MapGet("/AdminOrManager",
            [Authorize(Roles = "Admin, Manager")] () =>
            {
                return "Admin Or Manager";
            });

            //Dành cho thành viên (khách hàng đã đăng ký tài khoản)
            app.MapGet("/CustomerAccess",
            [Authorize(Roles = "Admin, Manager, Customer")] () =>
            {
                return "Customer";
            });

            //Guest (truy cập thông tin chung)
            app.MapGet("/GuestAccess", () => "Guest Access: Public Cinema Information")
               .AllowAnonymous();
            return app;
        }
        [Authorize(Roles = "Admin")]
        private static string AdminOnly()
        {
            return "Admin Only";
        }
    }
}
