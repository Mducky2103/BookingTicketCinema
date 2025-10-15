using Microsoft.AspNetCore.Authorization;

namespace BookingTicketCinema.Controllers
{
    public static class AuthorizationDemoEndpoints
    {
        public static IEndpointRouteBuilder MapAuthorizationDemoEndpoints(this IEndpointRouteBuilder app)
        {
            //Chỉ dành cho admin quản lý hệ thống
            app.MapGet("/AdminOnly", AdminOnly);

            //Dành cho admin và nhân viên rạp phim
            app.MapGet("/AdminOrStaff",
            [Authorize(Roles = "Admin, Staff")] () =>
            {
                return "Admin Or Staff";
            });

            //Dành cho tất cả thành viên (admin, nhân viên, khách hàng)
            app.MapGet("/MemberAccess",
            [Authorize(Roles = "Admin, Staff, Customer")] () =>
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
