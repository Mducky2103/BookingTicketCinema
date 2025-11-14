using BookingTicketCinema.Models;

namespace BookingTicketCinema.Extensions
{
    public static class AppConfigExtensions
    {
        public static WebApplication ConfigureCORS(
            this WebApplication app,
            IConfiguration config)
        {
            app.UseCors("AllowSpecificOrigins");
            return app;
        }

        public static IServiceCollection AddAppConfig(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.Configure<AppSettings>(config.GetSection("AppSettings"));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", policyBuilder =>
                {
                    // Lấy URL từ appsettings.json (an toàn hơn)
                    var clientWebAppUrl = config["ClientUrls:WebApp"];
                    var clientAdminUrl = config["ClientUrls:ManagementApp"];

                    // (Kiểm tra null nếu cần)
                    if (clientWebAppUrl != null && clientAdminUrl != null)
                    {
                        policyBuilder.WithOrigins(clientWebAppUrl, clientAdminUrl)
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    }
                });
            });
            return services;
        }
    }
}
