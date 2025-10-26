using BookingTicketCinema.DTO;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services.Interface;

namespace BookingTicketCinema.Services
{
    public class SeatGroupService : ISeatGroupService
    {
        private readonly ISeatGroupRepository _seatGroupRepository;
        private readonly IRoomRepository _roomRepository;

        public SeatGroupService(ISeatGroupRepository seatGroupRepository, IRoomRepository roomRepository)
        {
            _seatGroupRepository = seatGroupRepository;
            _roomRepository = roomRepository;
        }

        public async Task<IEnumerable<SeatGroupResponseDto>> GetAllAsync()
        {
            var seatGroups = await _seatGroupRepository.GetAllAsync();
            return seatGroups.Select(sg => new SeatGroupResponseDto
            {
                SeatGroupId = sg.SeatGroupId,
                GroupName = sg.GroupName,
                Type = sg.Type,
                RoomId = sg.RoomId,
                CreatedAt = sg.CreatedAt,
                UpdatedAt = sg.UpdatedAt
            });
        }

        public async Task<SeatGroupResponseDto?> GetByIdAsync(int id)
        {
            var seatGroup = await _seatGroupRepository.GetByIdAsync(id);
            if (seatGroup == null) return null;

            return new SeatGroupResponseDto
            {
                SeatGroupId = seatGroup.SeatGroupId,
                GroupName = seatGroup.GroupName,
                Type = seatGroup.Type,
                RoomId = seatGroup.RoomId,
                CreatedAt = seatGroup.CreatedAt,
                UpdatedAt = seatGroup.UpdatedAt
            };
        }

        public async Task<IEnumerable<SeatGroupResponseDto>> GetByRoomIdAsync(int roomId)
        {
            var seatGroups = await _seatGroupRepository.GetByRoomIdAsync(roomId);
            return seatGroups.Select(sg => new SeatGroupResponseDto
            {
                SeatGroupId = sg.SeatGroupId,
                GroupName = sg.GroupName,
                Type = sg.Type,
                RoomId = sg.RoomId,
                CreatedAt = sg.CreatedAt,
                UpdatedAt = sg.UpdatedAt
            });
        }

        public async Task<SeatGroupResponseDto> CreateAsync(CreateSeatGroupDto dto)
        {
            var room = await _roomRepository.GetByIdAsync(dto.RoomId);
            if (room == null) throw new Exception("Room not found");

            var seatGroup = new SeatGroup
            {
                GroupName = dto.GroupName,
                Type = dto.Type,
                RoomId = dto.RoomId
            };

            await _seatGroupRepository.AddAsync(seatGroup);
            await _seatGroupRepository.SaveChangesAsync();

            return new SeatGroupResponseDto
            {
                SeatGroupId = seatGroup.SeatGroupId,
                GroupName = seatGroup.GroupName,
                Type = seatGroup.Type,
                RoomId = seatGroup.RoomId,
                CreatedAt = seatGroup.CreatedAt,
                UpdatedAt = seatGroup.UpdatedAt
            };
        }

        public async Task<SeatGroupResponseDto?> UpdateAsync(int id, UpdateSeatGroupDto dto)
        {
            var seatGroup = await _seatGroupRepository.GetByIdAsync(id);
            if (seatGroup == null) return null;

            if (dto.GroupName != null) seatGroup.GroupName = dto.GroupName;
            if (dto.Type.HasValue) seatGroup.Type = dto.Type.Value;

            await _seatGroupRepository.UpdateAsync(seatGroup);
            await _seatGroupRepository.SaveChangesAsync();

            return new SeatGroupResponseDto
            {
                SeatGroupId = seatGroup.SeatGroupId,
                GroupName = seatGroup.GroupName,
                Type = seatGroup.Type,
                RoomId = seatGroup.RoomId,
                CreatedAt = seatGroup.CreatedAt,
                UpdatedAt = seatGroup.UpdatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var seatGroup = await _seatGroupRepository.GetByIdAsync(id);
            if (seatGroup == null) return false;

            await _seatGroupRepository.DeleteAsync(seatGroup);
            await _seatGroupRepository.SaveChangesAsync();
            return true;
        }
    }
}


