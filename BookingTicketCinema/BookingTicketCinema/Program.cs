using BookingTicketCinema.Models;
using BookingTicketCinema.Services;
using BookingTicketCinema.Controllers;
using BookingTicketCinema.Extensions;
using BookingTicketCinema.Services.Interface;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Repositories;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.OData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add OData support and register EDM model under the "odata" route prefix.
builder.Services.AddControllers()
    .AddOData(opt => opt.AddRouteComponents("odata", GetEdmModel())
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .SetMaxTop(100)
        .Count());
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

builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();

var app = builder.Build();

// Build EDM model for OData
static IEdmModel GetEdmModel()
{
    var odataBuilder = new ODataConventionModelBuilder();
    // register entity sets exposed via OData
    odataBuilder.EntitySet<Movie>("Movies");
    odataBuilder.EntitySet<Showtime>("Showtimes");
    return odataBuilder.GetEdmModel();
}

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