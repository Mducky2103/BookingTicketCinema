using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly CinemaDbContext _context;

        public SeatRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Seat>> GetAllAsync() =>
            await _context.Seats.ToListAsync();

        public async Task<Seat?> GetByIdAsync(int id) =>
            await _context.Seats.FindAsync(id);

        public async Task<IEnumerable<Seat>> GetByRoomIdAsync(int roomId) =>
            await _context.Seats.Where(s => s.RoomId == roomId).ToListAsync();

        public async Task AddAsync(Seat seat) =>
            await _context.Seats.AddAsync(seat);

        public async Task UpdateAsync(Seat seat) =>
            _context.Seats.Update(seat);

        public async Task DeleteAsync(Seat seat) =>
            _context.Seats.Remove(seat);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}


