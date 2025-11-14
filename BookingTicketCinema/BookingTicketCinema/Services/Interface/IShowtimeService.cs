using BookingTicketCinema.DTO;

namespace BookingTicketCinema.Services.Interface
{
    public interface IShowtimeService
    {
        Task<ShowtimeResponseDto> CreateAsync(ShowTimeCreateDto dto);
        Task<ShowtimeResponseDto?> UpdateAsync(int id, ShowTimeUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ShowtimeResponseDto>> GetAllAsync();
        Task<ShowtimeResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<ShowtimeResponseDto>> GetByRoomIdAsync(int roomId);
        Task<IEnumerable<ShowtimeDetailDto>> GetByMovieIdAsync(int movieId);
    }
}


