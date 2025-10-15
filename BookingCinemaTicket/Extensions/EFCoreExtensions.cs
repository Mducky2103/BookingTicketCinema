using BookingCinemaTicket.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace BookingCinemaTicket.Extensions
{
    public static class EFCoreExtensions
    {
        public static IServiceCollection InjectDbContext(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddDbContext<CinemaDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("conn")));
            return services;
        }
    }
}
