using BookingTicketCinema.Models;

namespace BookingTicketCinema.Repositories.Interface
{
    public interface ISeatGroupRepository
    {
        Task<IEnumerable<SeatGroup>> GetAllAsync();
        Task<SeatGroup?> GetByIdAsync(int id);
        Task<IEnumerable<SeatGroup>> GetByRoomIdAsync(int roomId);
        Task AddAsync(SeatGroup seatGroup);
        Task UpdateAsync(SeatGroup seatGroup);
        Task DeleteAsync(SeatGroup seatGroup);
        Task SaveChangesAsync();
    }
}

