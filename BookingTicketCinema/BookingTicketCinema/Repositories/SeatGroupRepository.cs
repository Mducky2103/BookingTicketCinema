using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Repositories
{
    public class SeatGroupRepository : ISeatGroupRepository
    {
        private readonly CinemaDbContext _context;

        public SeatGroupRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SeatGroup>> GetAllAsync() =>
            await _context.SeatGroups.ToListAsync();

        public async Task<SeatGroup?> GetByIdAsync(int id) =>
            await _context.SeatGroups.FindAsync(id);

        public async Task<IEnumerable<SeatGroup>> GetByRoomIdAsync(int roomId) =>
            await _context.SeatGroups.Where(sg => sg.RoomId == roomId).ToListAsync();

        public async Task AddAsync(SeatGroup seatGroup) =>
            await _context.SeatGroups.AddAsync(seatGroup);

        public async Task UpdateAsync(SeatGroup seatGroup) =>
            _context.SeatGroups.Update(seatGroup);

        public async Task DeleteAsync(SeatGroup seatGroup) =>
            _context.SeatGroups.Remove(seatGroup);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }
}


