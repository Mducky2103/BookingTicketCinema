using BookingTicketCinema.Data;
using BookingTicketCinema.DTO;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IShowtimeRepository _showtimeRepository;
        private readonly CinemaDbContext _context;

        public ShowtimeService(IShowtimeRepository showtimeRepository, CinemaDbContext context)
        {
            _showtimeRepository = showtimeRepository;
            _context = context;
        }

        // ===================== GET ALL =====================
        public async Task<IEnumerable<ShowtimeResponseDto>> GetAllAsync()
        {
            var showtimes = await _showtimeRepository.GetAllAsync();
            return showtimes.Select(MapToResponseDto);
        }

        // ===================== CREATE =====================
        public async Task<ShowtimeResponseDto> CreateAsync(ShowTimeCreateDto dto)
        {
            // 1️⃣ Validate thời gian
            if (dto.StartTime <= DateTime.Now)
                throw new InvalidOperationException("StartTime phải nằm trong tương lai.");
            if (dto.EndTime <= dto.StartTime)
                throw new InvalidOperationException("EndTime phải sau StartTime.");

            // 2️⃣ Validate Movie và Room có tồn tại
            var movieExists = await _context.Movies.AnyAsync(m => m.MovieId == dto.MovieId);
            if (!movieExists)
                throw new InvalidOperationException($"MovieId {dto.MovieId} không tồn tại.");

            var roomExists = await _context.Rooms.AnyAsync(r => r.RoomId == dto.RoomId);
            if (!roomExists)
                throw new InvalidOperationException($"RoomId {dto.RoomId} không tồn tại.");

            // 3️⃣ Check trùng lịch trong cùng phòng
            var overlap = await _context.Showtimes
                .AnyAsync(s => s.RoomId == dto.RoomId &&
                               ((dto.StartTime >= s.StartTime && dto.StartTime < s.EndTime) ||
                                (dto.EndTime > s.StartTime && dto.EndTime <= s.EndTime) ||
                                (dto.StartTime <= s.StartTime && dto.EndTime >= s.EndTime)));
            if (overlap)
                throw new InvalidOperationException("Phòng này đã có suất chiếu trùng giờ.");

            // 4️⃣ Tạo mới
            var showtime = new Showtime
            {
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                MovieId = dto.MovieId,
                RoomId = dto.RoomId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _showtimeRepository.AddAsync(showtime);
            await _showtimeRepository.SaveChangesAsync();

            // 5️⃣ Load lại entity kèm Movie & Room để trả về
            var reloaded = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstAsync(s => s.ShowtimeId == showtime.ShowtimeId);

            return MapToResponseDto(reloaded);
        }

        // ===================== UPDATE =====================
        public async Task<ShowtimeResponseDto?> UpdateAsync(int id, ShowTimeUpdateDto dto)
        {
            var showtime = await _showtimeRepository.GetByIdAsync(id);
            if (showtime == null)
                throw new InvalidOperationException("Không tìm thấy showtime.");

            // 1️⃣ Validate thời gian
            if (dto.StartTime.HasValue && dto.StartTime.Value <= DateTime.Now)
                throw new InvalidOperationException("StartTime phải nằm trong tương lai.");

            if (dto.EndTime.HasValue && dto.StartTime.HasValue && dto.EndTime <= dto.StartTime)
                throw new InvalidOperationException("EndTime phải sau StartTime.");

            // 2️⃣ Validate Movie và Room tồn tại
            if (dto.MovieId.HasValue)
            {
                var movieExists = await _context.Movies.AnyAsync(m => m.MovieId == dto.MovieId.Value);
                if (!movieExists)
                    throw new InvalidOperationException($"MovieId {dto.MovieId.Value} không tồn tại.");
            }

            if (dto.RoomId.HasValue)
            {
                var roomExists = await _context.Rooms.AnyAsync(r => r.RoomId == dto.RoomId.Value);
                if (!roomExists)
                    throw new InvalidOperationException($"RoomId {dto.RoomId.Value} không tồn tại.");
            }

            // 3️⃣ Check trùng lịch nếu có đổi phòng hoặc giờ
            var newRoomId = dto.RoomId ?? showtime.RoomId;
            var newStart = dto.StartTime ?? showtime.StartTime;
            var newEnd = dto.EndTime ?? showtime.EndTime;

            var overlap = await _context.Showtimes
                .AnyAsync(s => s.ShowtimeId != id &&
                               s.RoomId == newRoomId &&
                               ((newStart >= s.StartTime && newStart < s.EndTime) ||
                                (newEnd > s.StartTime && newEnd <= s.EndTime) ||
                                (newStart <= s.StartTime && newEnd >= s.EndTime)));
            if (overlap)
                throw new InvalidOperationException("Phòng này đã có suất chiếu trùng giờ.");

            // 4️⃣ Cập nhật
            showtime.StartTime = newStart;
            showtime.EndTime = newEnd;
            showtime.MovieId = dto.MovieId ?? showtime.MovieId;
            showtime.RoomId = newRoomId;
            showtime.UpdatedAt = DateTime.UtcNow;

            await _showtimeRepository.UpdateAsync(showtime);
            await _showtimeRepository.SaveChangesAsync();

            var reloaded = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .FirstAsync(s => s.ShowtimeId == id);

            return MapToResponseDto(reloaded);
        }

        // ===================== DELETE =====================
        public async Task<bool> DeleteAsync(int id)
        {
            var showtime = await _showtimeRepository.GetByIdAsync(id);
            if (showtime == null) return false;

            await _showtimeRepository.DeleteAsync(showtime);
            await _showtimeRepository.SaveChangesAsync();
            return true;
        }

        // ===================== GET BY ID =====================
        public async Task<ShowtimeResponseDto?> GetByIdAsync(int id)
        {
            var showtime = await _showtimeRepository.GetByIdAsync(id);
            return showtime == null ? null : MapToResponseDto(showtime);
        }

        // ===================== GET BY ROOM =====================
        public async Task<IEnumerable<ShowtimeResponseDto>> GetByRoomIdAsync(int roomId)
        {
            var showtimes = await _showtimeRepository.GetByRoomIdAsync(roomId);
            return showtimes.Select(MapToResponseDto);
        }

        // ===================== Helper =====================
        private static ShowtimeResponseDto MapToResponseDto(Showtime s)
        {
            return new ShowtimeResponseDto
            {
                ShowtimeId = s.ShowtimeId,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                MovieId = s.MovieId,
                MovieName = s.Movie?.Title,
                RoomId = s.RoomId,
                RoomName = s.Room?.Name,
                PriceRules = s.PriceRules?.Select(pr => new PriceRuleResponseDto
                {
                    PriceRuleId = pr.PriceRuleId,
                    BasePrice = pr.BasePrice,
                    DayOfWeek = pr.DayOfWeek,
                    SeatGroupId = pr.SeatGroupId,
                    Slot = pr.Slot,
                    ShowtimeId = pr.ShowtimeId,
                    CreatedAt = pr.CreatedAt,
                    UpdatedAt = pr.UpdatedAt
                }).ToList(),
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            };
        }
    }
}
