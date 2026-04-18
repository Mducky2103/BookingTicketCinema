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

        // Hàm tạo hàng loạt trả về Tuple để biết cái nào thành công, cái nào lỗi
        Task<(List<ShowtimeResponseDto> Success, List<string> Errors)> CreateBulkAsync(ShowTimeBulkCreateDto dto);

        // Hàm check nhanh để báo lỗi ngay trên Form
        Task<bool> IsOverlapAsync(int roomId, DateTime startTime, int movieId);
    }
}


