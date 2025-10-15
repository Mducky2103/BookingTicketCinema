using BookingCinemaTicket.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookingCinemaTicket.Data
{
    public class CinemaDbContext : IdentityDbContext<User>
    {
        public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
