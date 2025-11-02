using BookingTicketCinema.Models;

namespace BookingTicketCinema.Repositories.Interface
{
    public interface IShowtimeRepository
    {
        Task<IEnumerable<Showtime>> GetAllAsync();
        Task<Showtime?> GetByIdAsync(int id);
        Task<IEnumerable<Showtime>> GetByRoomIdAsync(int roomId);
        Task AddAsync(Showtime showtime);
        Task UpdateAsync(Showtime showtime);
        Task DeleteAsync(Showtime showtime);
        Task SaveChangesAsync();
    }
}


