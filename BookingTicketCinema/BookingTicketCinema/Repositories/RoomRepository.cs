using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly CinemaDbContext _context;

        public RoomRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllAsync() =>
            await _context.Rooms.ToListAsync();

        public async Task<Room?> GetByIdAsync(int id) =>
            await _context.Rooms.FindAsync(id);

        public async Task AddAsync(Room room) =>
            await _context.Rooms.AddAsync(room);

        public async Task UpdateAsync(Room room) =>
            _context.Rooms.Update(room);

        public async Task DeleteAsync(Room room) =>
            _context.Rooms.Remove(room);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}


