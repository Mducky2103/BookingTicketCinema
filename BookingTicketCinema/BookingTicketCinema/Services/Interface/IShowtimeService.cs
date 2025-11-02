using BookingTicketCinema.DTO;

namespace BookingTicketCinema.Services.Interface
{
    public interface IShowtimeService
    {
        Task<IEnumerable<ShowtimeResponseDto>> GetAllAsync();
        Task<ShowtimeResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<ShowtimeResponseDto>> GetByRoomIdAsync(int roomId);
    }
}


