using BookingTicketCinema.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Extensions
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
