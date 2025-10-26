using BookingTicketCinema.DTO;

namespace BookingTicketCinema.Services.Interface
{
    public interface ISeatGroupService
    {
        Task<IEnumerable<SeatGroupResponseDto>> GetAllAsync();
        Task<SeatGroupResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<SeatGroupResponseDto>> GetByRoomIdAsync(int roomId);
        Task<SeatGroupResponseDto> CreateAsync(CreateSeatGroupDto dto);
        Task<SeatGroupResponseDto?> UpdateAsync(int id, UpdateSeatGroupDto dto);
        Task<bool> DeleteAsync(int id);
    }
}

