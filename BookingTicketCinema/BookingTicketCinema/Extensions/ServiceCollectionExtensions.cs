using BookingTicketCinema.Repositories;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services;
using BookingTicketCinema.Services.Interface;

namespace BookingTicketCinema.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<EmailService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IRoomService, RoomService>();

            services.AddScoped<ISeatGroupRepository, SeatGroupRepository>();
            services.AddScoped<ISeatGroupService, SeatGroupService>();

            services.AddScoped<ISeatRepository, SeatRepository>();
            services.AddScoped<ISeatService, SeatService>();

            services.AddScoped<IShowtimeRepository, ShowtimeRepository>();
            services.AddScoped<IShowtimeService, ShowtimeService>();

            services.AddScoped<IPriceRuleRepository, PriceRuleRepository>();
            services.AddScoped<IPriceRuleService, PriceRuleService>();

            services.AddScoped<IMovieRepository, MovieRepository>();
            services.AddScoped<IMovieService, MovieService>();

            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<ITicketService, TicketService>();

            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentService, PaymentService>();

            services.AddScoped<IPOSService, POSService>();

            services.AddScoped<IStatisticsRepository, StatisticsRepository>();
            services.AddScoped<IStatisticsService, StatisticsService>();

            return services;
        }
    }
}
