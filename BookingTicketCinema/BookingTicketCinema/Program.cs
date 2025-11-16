using BookingTicketCinema.Controllers;
using BookingTicketCinema.Extensions;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services;
using BookingTicketCinema.Services.Interface;
using Hangfire;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

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

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("conn"))); 

builder.Services.AddHangfireServer();
builder.Services.AddScoped<IBackgroundJobService, BackgroundJobService>();
builder.Services.AddApplicationServices();

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
app.UseHttpsRedirection();
app.UseStaticFiles();

app.ConfigureSwaggerExplorer()
               .ConfigureCORS(builder.Configuration)
               .AddIdentityAuthMiddlewares();

app.MapControllers();
app.UseHangfireDashboard("/hangfire"); 

RecurringJob.AddOrUpdate<IBackgroundJobService>(
    "job-id-don-dep-ve-treo", 
    service => service.CancelExpiredPaymentsAsync(), 
    Cron.MinuteInterval(2) 
);
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