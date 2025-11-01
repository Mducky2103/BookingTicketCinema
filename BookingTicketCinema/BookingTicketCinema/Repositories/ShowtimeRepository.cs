using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Repositories
{
    public class ShowtimeRepository : IShowtimeRepository
    {
        private readonly CinemaDbContext _context;

        public ShowtimeRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Showtime>> GetAllAsync() =>
            await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .ToListAsync();

        public async Task<Showtime?> GetByIdAsync(int id) =>
            await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.ShowtimeId == id);

        public async Task<IEnumerable<Showtime>> GetByRoomIdAsync(int roomId) =>
            await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .Where(s => s.RoomId == roomId)
                .ToListAsync();

        public async Task AddAsync(Showtime showtime) =>
            await _context.Showtimes.AddAsync(showtime);

        public async Task UpdateAsync(Showtime showtime) =>
            _context.Showtimes.Update(showtime);

        public async Task DeleteAsync(Showtime showtime) =>
            _context.Showtimes.Remove(showtime);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}


