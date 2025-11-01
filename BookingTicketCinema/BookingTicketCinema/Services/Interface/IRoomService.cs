using BookingTicketCinema.DTO;

namespace BookingTicketCinema.Services.Interface
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomResponseDto>> GetAllAsync();
        Task<RoomResponseDto?> GetByIdAsync(int id);
        Task<RoomResponseDto> CreateAsync(CreateRoomDto dto);
        Task<RoomResponseDto?> UpdateAsync(int id, UpdateRoomDto dto);
        Task<bool> DeleteAsync(int id);
    }
}


