using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using static BookingTicketCinema.Models.Ticket;

namespace BookingTicketCinema.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly CinemaDbContext _context;

        public TicketRepository(CinemaDbContext context) { _context = context; }

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _context.Tickets.FindAsync(id);
        }

        public async Task AddRangeAsync(IEnumerable<Ticket> tickets)
        {
            _context.Tickets.AddRange(tickets);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Ticket>> GetByUserIdAsync(string userId)
        {
            // Query gốc của bạn từ TicketManagement
            return await _context.Tickets
                .Include(t => t.Showtime).ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetByShowtimeIdAsync(int showtimeId)
        {
            // Query gốc của bạn từ BookingController
            return await _context.Tickets
                .Where(t => t.ShowtimeId == showtimeId && t.Status != TicketStatus.Cancelled)
                .ToListAsync();
        }
    }
}
