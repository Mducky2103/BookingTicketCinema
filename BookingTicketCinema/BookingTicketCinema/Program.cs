using BookingTicketCinema.Models;
using BookingTicketCinema.Services;
using BookingTicketCinema.Controllers;
using BookingTicketCinema.Extensions;
using BookingTicketCinema.Services.Interface;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Repositories;

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

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();

builder.Services.AddScoped<ISeatGroupRepository, SeatGroupRepository>();
builder.Services.AddScoped<ISeatGroupService, SeatGroupService>();

builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<ISeatService, SeatService>();

builder.Services.AddScoped<IShowtimeRepository, ShowtimeRepository>();
builder.Services.AddScoped<IShowtimeService, ShowtimeService>();

builder.Services.AddScoped<IPriceRuleRepository, PriceRuleRepository>();
builder.Services.AddScoped<IPriceRuleService, PriceRuleService>();

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
    .MapAuthorizationDemoEndpoints()
    .MapRoomEndpoints()
    .MapSeatGroupEndpoints()
    .MapSeatEndpoints()
    .MapShowtimeEndpoints()
    .MapPriceRuleEndpoints();

// Seed database with demo admin account
await app.SeedDatabaseAsync();

await app.RunAsync();