using BookingTicketCinema.Models;

namespace BookingTicketCinema.Repositories.Interface
{
    public interface ISeatRepository
    {
        Task<IEnumerable<Seat>> GetAllAsync();
        Task<Seat?> GetByIdAsync(int id);
        Task<IEnumerable<Seat>> GetByRoomIdAsync(int roomId);
        Task AddAsync(Seat seat);
        Task UpdateAsync(Seat seat);
        Task DeleteAsync(Seat seat);
        Task SaveChangesAsync();
    }
}


