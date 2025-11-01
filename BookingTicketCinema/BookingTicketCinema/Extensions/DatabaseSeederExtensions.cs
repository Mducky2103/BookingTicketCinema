using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Extensions
{
    public static class DatabaseSeederExtensions
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<CinemaDbContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Ensure database is created and migrations are applied
                await context.Database.MigrateAsync();

                // Create roles if they don't exist
                string[] roles = { "Admin", "Staff", "Customer" };
                foreach (var roleName in roles)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Create demo admin user if it doesn't exist
                const string adminEmail = "admin@demo.com";
                const string adminPassword = "Admin123!";
                const string adminFullName = "Demo Admin";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new User
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        FullName = adminFullName,
                        Gender = "Male",
                        DOB = DateOnly.FromDateTime(DateTime.Now.AddYears(-30))
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        Console.WriteLine($"✅ Demo admin account created successfully!");
                        Console.WriteLine($"   Email: {adminEmail}");
                        Console.WriteLine($"   Password: {adminPassword}");
                    }
                    else
                    {
                        Console.WriteLine($"❌ Failed to create demo admin account:");
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"   - {error.Description}");
                        }
                    }
                }
                else
                {
                    // Ensure admin user has Admin role
                    var userRoles = await userManager.GetRolesAsync(adminUser);
                    if (!userRoles.Contains("Admin"))
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        Console.WriteLine($"✅ Admin role assigned to existing user: {adminEmail}");
                    }
                    else
                    {
                        Console.WriteLine($"ℹ️  Demo admin account already exists: {adminEmail}");
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}

