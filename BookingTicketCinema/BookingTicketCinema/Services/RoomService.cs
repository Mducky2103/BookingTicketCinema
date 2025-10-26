using BookingTicketCinema.DTO;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services.Interface;

namespace BookingTicketCinema.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<IEnumerable<RoomResponseDto>> GetAllAsync()
        {
            var rooms = await _roomRepository.GetAllAsync();
            return rooms.Select(r => new RoomResponseDto
            {
                RoomId = r.RoomId,
                Name = r.Name,
                Capacity = r.Capacity,
                Type = r.Type,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            });
        }

        public async Task<RoomResponseDto?> GetByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return null;

            return new RoomResponseDto
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Capacity = room.Capacity,
                Type = room.Type,
                CreatedAt = room.CreatedAt,
                UpdatedAt = room.UpdatedAt
            };
        }

        public async Task<RoomResponseDto> CreateAsync(CreateRoomDto dto)
        {
            var room = new Room
            {
                Name = dto.Name,
                Capacity = dto.Capacity,
                Type = dto.Type
            };

            await _roomRepository.AddAsync(room);
            await _roomRepository.SaveChangesAsync();

            return new RoomResponseDto
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Capacity = room.Capacity,
                Type = room.Type,
                CreatedAt = room.CreatedAt,
                UpdatedAt = room.UpdatedAt
            };
        }

        public async Task<RoomResponseDto?> UpdateAsync(int id, UpdateRoomDto dto)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return null;

            if (dto.Name != null) room.Name = dto.Name;
            if (dto.Capacity.HasValue) room.Capacity = dto.Capacity.Value;
            if (dto.Type.HasValue) room.Type = dto.Type.Value;

            await _roomRepository.UpdateAsync(room);
            await _roomRepository.SaveChangesAsync();

            return new RoomResponseDto
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Capacity = room.Capacity,
                Type = room.Type,
                CreatedAt = room.CreatedAt,
                UpdatedAt = room.UpdatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return false;

            await _roomRepository.DeleteAsync(room);
            await _roomRepository.SaveChangesAsync();
            return true;
        }
    }
}


