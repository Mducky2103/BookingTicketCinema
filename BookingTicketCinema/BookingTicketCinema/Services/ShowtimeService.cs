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
        private const int CLEANING_MINUTES = 15;
        public async Task<ShowtimeResponseDto> CreateAsync(ShowTimeCreateDto dto)
        {
            // 1️⃣ Validate thời gian
            if (dto.StartTime <= DateTime.Now)
                throw new InvalidOperationException("StartTime phải nằm trong tương lai.");
            if (dto.EndTime <= dto.StartTime)
                throw new InvalidOperationException("EndTime phải sau StartTime.");

            // 2️⃣ Validate Movie và Room có tồn tại
            //var movieExists = await _context.Movies.AnyAsync(m => m.MovieId == dto.MovieId);
            //if (!movieExists)
            //    throw new InvalidOperationException($"MovieId {dto.MovieId} không tồn tại.");
            var movie = await _context.Movies.FindAsync(dto.MovieId);
            if (movie == null)
                throw new InvalidOperationException($"MovieId {dto.MovieId} không tồn tại.");

            var roomExists = await _context.Rooms.AnyAsync(r => r.RoomId == dto.RoomId);
            if (!roomExists)
                throw new InvalidOperationException($"RoomId {dto.RoomId} không tồn tại.");

            // Tự động tính ENDTIME
            var calculatedEndTime = dto.StartTime.Add(movie.Duration).AddMinutes(CLEANING_MINUTES);

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
                EndTime = calculatedEndTime,
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

        // ===================== CREATE BULK (TẠO HÀNG LOẠT) =====================
        public async Task<(List<ShowtimeResponseDto> Success, List<string> Errors)> CreateBulkAsync(ShowTimeBulkCreateDto dto)
        {
            // Kiểm tra phim để lấy Duration
            var movie = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.MovieId == dto.MovieId);
            if (movie == null)
                return (new List<ShowtimeResponseDto>(), new List<string> { "Phim không tồn tại." });

            var roomExists = await _context.Rooms.AnyAsync(r => r.RoomId == dto.RoomId);
            if (!roomExists)
                return (new List<ShowtimeResponseDto>(), new List<string> { "Phòng không tồn tại." });

            var successEntities = new List<Showtime>();
            var errors = new List<string>();

            foreach (var startTime in dto.StartTimes)
            {
                // Kiểm tra suất chiếu trong tương lai
                if (startTime <= DateTime.Now)
                {
                    errors.Add($"Suất chiếu lúc {startTime:HH:mm} không hợp lệ (phải ở tương lai).");
                    continue;
                }

                // 2. Tự động tính EndTime = StartTime + Duration + Cleaning
                var endTime = startTime.Add(movie.Duration).AddMinutes(CLEANING_MINUTES);

                // 3. Kiểm tra trùng lịch (Overlap)
                // Công thức chuẩn: (Start1 < End2) AND (End1 > Start2)
                var overlap = await _context.Showtimes
                    .AsNoTracking()
                    .AnyAsync(s => s.RoomId == dto.RoomId &&
                                   startTime < s.EndTime &&
                                   endTime > s.StartTime);

                if (overlap)
                {
                    errors.Add($"Suất chiếu {startTime:HH:mm} - {endTime:HH:mm} bị trùng lịch với suất khác trong phòng.");
                    continue;
                }

                // Kiểm tra trùng lặp ngay trong danh sách đang chuẩn bị thêm (nếu Admin nhập 2 giờ sát nhau)
                var internalOverlap = successEntities.Any(s => startTime < s.EndTime && endTime > s.StartTime);
                if (internalOverlap)
                {
                    errors.Add($"Suất chiếu {startTime:HH:mm} bị trùng với một suất khác trong cùng danh sách tạo mới.");
                    continue;
                }

                successEntities.Add(new Showtime
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    MovieId = dto.MovieId,
                    RoomId = dto.RoomId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // 4. Lưu vào DB
            if (successEntities.Any())
            {
                await _context.Showtimes.AddRangeAsync(successEntities);
                await _context.SaveChangesAsync();
            }

            // 5. Trả về kết quả (Dùng MapToResponseDto có sẵn của bạn)
            // Lưu ý: Cần Include thêm Movie/Room để MapToResponseDto không bị Null tên
            var resultIds = successEntities.Select(s => s.ShowtimeId).ToList();
            var reloadedSuccess = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .Where(s => resultIds.Contains(s.ShowtimeId))
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            return (reloadedSuccess.Select(MapToResponseDto).ToList(), errors);
        }

        // ===================== CHECK OVERLAP (DÙNG CHO BÁO LỖI TẠI CHỖ) =====================
        public async Task<bool> IsOverlapAsync(int roomId, DateTime startTime, int movieId)
        {
            var movie = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.MovieId == movieId);
            if (movie == null) return true;

            var endTime = startTime.Add(movie.Duration).AddMinutes(CLEANING_MINUTES);

            return await _context.Showtimes.AnyAsync(s =>
                s.RoomId == roomId && startTime < s.EndTime && endTime > s.StartTime);
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

        public async Task<IEnumerable<ShowtimeDetailDto>> GetByMovieIdAsync(int movieId)
        {
            var showtimes = await _showtimeRepository.GetByMovieIdAsync(movieId);

            // Chuyển từ Model (Showtime) sang DTO
            return showtimes.Select(s => new ShowtimeDetailDto
            {
                ShowtimeId = s.ShowtimeId,
                StartTime = s.StartTime,
                RoomName = s.Room.Name,
                RoomType = (int)s.Room.Type
            });
        }
    }
}
