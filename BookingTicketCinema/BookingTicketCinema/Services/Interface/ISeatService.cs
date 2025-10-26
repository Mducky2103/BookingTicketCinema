using BookingTicketCinema.DTO;

namespace BookingTicketCinema.Services.Interface
{
    public interface ISeatService
    {
        Task<IEnumerable<SeatResponseDto>> GetAllAsync();
        Task<SeatResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<SeatResponseDto>> GetByRoomIdAsync(int roomId);
        Task<SeatResponseDto> CreateAsync(CreateSeatDto dto);
        Task<SeatResponseDto?> UpdateAsync(int id, UpdateSeatDto dto);
        Task<bool> DeleteAsync(int id);
    }
}


