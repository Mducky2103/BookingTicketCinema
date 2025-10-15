using BookingTicketCinema.Models;
using BookingTicketCinema.Services;
using BookingTicketCinema.Controllers;
using BookingTicketCinema.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerExplorer()
                            .InjectDbContext(builder.Configuration)
                            .AddAppConfig(builder.Configuration)
                            .AddIdentityHandlersAndStores()
                            .ConfigureIdentityOptions()
                            .AddIdentityAuth(builder.Configuration);
builder.Services.AddSingleton<EmailService>();

var app = builder.Build();

app.ConfigureSwaggerExplorer()
               .ConfigureCORS(builder.Configuration)
               .AddIdentityAuthMiddlewares();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapControllers();

app.MapGroup("/api")
    .MapIdentityApi<User>();

app.MapGroup("/api")
    .MapIdentityUserEndpoints()
    .MapAccountEndpoints()
    .MapAuthorizationDemoEndpoints();
app.Run();