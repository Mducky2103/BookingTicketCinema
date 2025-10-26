using BookingTicketCinema.Models;

namespace BookingTicketCinema.Repositories.Interface
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllAsync();
        Task<Room?> GetByIdAsync(int id);
        Task AddAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(Room room);
        Task SaveChangesAsync();
    }
}


