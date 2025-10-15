using BookingTicketCinema.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Data
{
    public class CinemaDbContext : IdentityDbContext<User>
    {
        public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<VoucherRedemption> VoucherRedemptions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<SeatGroup> SeatGroups { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<PriceRule> PriceRules { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // ---- Room <> SeatGroup <> Seat relationships ----
            // Room -> SeatGroups : RESTRICT (avoid multiple cascade paths)
            builder.Entity<SeatGroup>()
                .HasOne(sg => sg.Room)
                .WithMany(r => r.SeatGroups)
                .HasForeignKey(sg => sg.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Room -> Seats : CASCADE (deleting a Room deletes its Seats)
            builder.Entity<Seat>()
                .HasOne(s => s.Room)
                .WithMany(r => r.Seats)
                .HasForeignKey(s => s.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // SeatGroup -> Seats : RESTRICT (do not cascade; prevents multiple cascade paths)
            builder.Entity<Seat>()
                .HasOne(s => s.SeatGroup)
                .WithMany(g => g.Seats)
                .HasForeignKey(s => s.SeatGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // If you want Seat.SeatGroupId nullable and auto-set null on delete:
            // .OnDelete(DeleteBehavior.SetNull) and make Seat.SeatGroupId int?

            // ---- Movie <> Showtime <> Ticket relationships ----
            // Movie -> Showtimes : CASCADE (deleting a Movie deletes its Showtimes)
            builder.Entity<Showtime>()
                .HasOne(st => st.Movie)
                .WithMany(m => m.Showtimes)
                .HasForeignKey(st => st.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            // Room -> Showtimes : RESTRICT (do not cascade)
            builder.Entity<Showtime>()
                .HasOne(st => st.Room)
                .WithMany(r => r.Showtimes)
                .HasForeignKey(st => st.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Showtime -> Tickets : CASCADE (deleting a Showtime deletes its Tickets)
            builder.Entity<Ticket>()
                .HasOne(t => t.Showtime)
                .WithMany(st => st.Tickets)
                .HasForeignKey(t => t.ShowtimeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ticket -> Seat : RESTRICT (seat should remain)
            builder.Entity<Ticket>()
                .HasOne(t => t.Seat)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => t.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- Ticket <-> Payment (1:1) ----
            builder.Entity<Ticket>()
                .HasOne(t => t.Payment)
                .WithOne(p => p.Ticket)
                .HasForeignKey<Payment>(p => p.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- Promotion <> VoucherRedemption ----
            builder.Entity<VoucherRedemption>()
                .HasOne(vr => vr.Promotion)
                .WithMany(p => p.VoucherRedemptions)
                .HasForeignKey(vr => vr.PromotionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- PriceRule relations ----
            // PriceRule may apply to SeatGroup (optional)
            builder.Entity<PriceRule>()
                .HasOne(pr => pr.SeatGroup)
                .WithMany(sg => sg.PriceRules)
                .HasForeignKey(pr => pr.SeatGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // PriceRule may apply to a specific Showtime (optional)
            builder.Entity<PriceRule>()
                .HasOne(pr => pr.Showtime)
                .WithMany(st => st.PriceRules)
                .HasForeignKey(pr => pr.ShowtimeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---- User <> Ticket and User <> VoucherRedemption (navigation existing on User) ----
            builder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<VoucherRedemption>()
                .HasOne(vr => vr.User)
                .WithMany(u => u.VoucherRedemptions)
                .HasForeignKey(vr => vr.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(builder);
        }
    }
}
