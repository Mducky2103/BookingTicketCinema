using BookingTicketCinema.Models;

namespace BookingTicketCinema.Repositories.Interface
{
    public interface ITicketRepository
    {
        Task<Ticket?> GetByIdAsync(int id);
        Task AddRangeAsync(IEnumerable<Ticket> tickets);
        Task UpdateAsync(Ticket ticket);

        // Lấy vé của 1 user (đã JOIN)
        Task<IEnumerable<Ticket>> GetByUserIdAsync(string userId);

        // Lấy vé của 1 suất chiếu (để kiểm tra)
        Task<IEnumerable<Ticket>> GetByShowtimeIdAsync(int showtimeId);
    }
}
