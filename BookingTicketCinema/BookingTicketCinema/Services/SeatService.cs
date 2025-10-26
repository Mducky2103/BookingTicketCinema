using BookingTicketCinema.DTO;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services.Interface;

namespace BookingTicketCinema.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ISeatGroupRepository _seatGroupRepository;

        public SeatService(ISeatRepository seatRepository, IRoomRepository roomRepository, ISeatGroupRepository seatGroupRepository)
        {
            _seatRepository = seatRepository;
            _roomRepository = roomRepository;
            _seatGroupRepository = seatGroupRepository;
        }

        public async Task<IEnumerable<SeatResponseDto>> GetAllAsync()
        {
            var seats = await _seatRepository.GetAllAsync();
            return seats.Select(s => new SeatResponseDto
            {
                SeatId = s.SeatId,
                SeatNumber = s.SeatNumber,
                Status = s.Status,
                RoomId = s.RoomId,
                SeatGroupId = s.SeatGroupId,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            });
        }

        public async Task<SeatResponseDto?> GetByIdAsync(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);
            if (seat == null) return null;

            return new SeatResponseDto
            {
                SeatId = seat.SeatId,
                SeatNumber = seat.SeatNumber,
                Status = seat.Status,
                RoomId = seat.RoomId,
                SeatGroupId = seat.SeatGroupId,
                CreatedAt = seat.CreatedAt,
                UpdatedAt = seat.UpdatedAt
            };
        }

        public async Task<IEnumerable<SeatResponseDto>> GetByRoomIdAsync(int roomId)
        {
            var seats = await _seatRepository.GetByRoomIdAsync(roomId);
            return seats.Select(s => new SeatResponseDto
            {
                SeatId = s.SeatId,
                SeatNumber = s.SeatNumber,
                Status = s.Status,
                RoomId = s.RoomId,
                SeatGroupId = s.SeatGroupId,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            });
        }

        public async Task<SeatResponseDto> CreateAsync(CreateSeatDto dto)
        {
            var room = await _roomRepository.GetByIdAsync(dto.RoomId);
            if (room == null) throw new Exception("Room not found");

            var seatGroup = await _seatGroupRepository.GetByIdAsync(dto.SeatGroupId);
            if (seatGroup == null) throw new Exception("SeatGroup not found");

            var seat = new Seat
            {
                SeatNumber = dto.SeatNumber,
                Status = dto.Status,
                RoomId = dto.RoomId,
                SeatGroupId = dto.SeatGroupId
            };

            await _seatRepository.AddAsync(seat);
            await _seatRepository.SaveChangesAsync();

            return new SeatResponseDto
            {
                SeatId = seat.SeatId,
                SeatNumber = seat.SeatNumber,
                Status = seat.Status,
                RoomId = seat.RoomId,
                SeatGroupId = seat.SeatGroupId,
                CreatedAt = seat.CreatedAt,
                UpdatedAt = seat.UpdatedAt
            };
        }

        public async Task<SeatResponseDto?> UpdateAsync(int id, UpdateSeatDto dto)
        {
            var seat = await _seatRepository.GetByIdAsync(id);
            if (seat == null) return null;

            if (dto.SeatNumber != null) seat.SeatNumber = dto.SeatNumber;
            if (dto.Status.HasValue) seat.Status = dto.Status.Value;
            if (dto.SeatGroupId.HasValue)
            {
                var seatGroup = await _seatGroupRepository.GetByIdAsync(dto.SeatGroupId.Value);
                if (seatGroup == null) throw new Exception("SeatGroup not found");
                seat.SeatGroupId = dto.SeatGroupId.Value;
            }

            await _seatRepository.UpdateAsync(seat);
            await _seatRepository.SaveChangesAsync();

            return new SeatResponseDto
            {
                SeatId = seat.SeatId,
                SeatNumber = seat.SeatNumber,
                Status = seat.Status,
                RoomId = seat.RoomId,
                SeatGroupId = seat.SeatGroupId,
                CreatedAt = seat.CreatedAt,
                UpdatedAt = seat.UpdatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var seat = await _seatRepository.GetByIdAsync(id);
            if (seat == null) return false;

            await _seatRepository.DeleteAsync(seat);
            await _seatRepository.SaveChangesAsync();
            return true;
        }
    }
}

