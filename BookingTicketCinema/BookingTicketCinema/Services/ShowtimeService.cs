using BookingTicketCinema.DTO;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services.Interface;

namespace BookingTicketCinema.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IShowtimeRepository _showtimeRepository;

        public ShowtimeService(IShowtimeRepository showtimeRepository)
        {
            _showtimeRepository = showtimeRepository;
        }

        public async Task<IEnumerable<ShowtimeResponseDto>> GetAllAsync()
        {
            var showtimes = await _showtimeRepository.GetAllAsync();
            return showtimes.Select(s => new ShowtimeResponseDto
            {
                ShowtimeId = s.ShowtimeId,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                MovieId = s.MovieId,
                MovieName = s.Movie?.Title,
                RoomId = s.RoomId,
                RoomName = s.Room?.Name,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            });
        }
        public async Task<ShowtimeResponseDto> CreateAsync(ShowTimeCreateDto dto)
        {
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
            return new ShowtimeResponseDto
            {
                ShowtimeId = showtime.ShowtimeId,
                StartTime = showtime.StartTime,
                EndTime = showtime.EndTime,
                MovieId = showtime.MovieId,
                MovieName = showtime.Movie?.Title,
                RoomId = showtime.RoomId,
                RoomName = showtime.Room?.Name,
                CreatedAt = showtime.CreatedAt,
                UpdatedAt = showtime.UpdatedAt
            };
        }
        public async Task<ShowtimeResponseDto?> UpdateAsync(int id, ShowTimeUpdateDto dto)
        {
            var showtime = await _showtimeRepository.GetByIdAsync(id);
            if (showtime == null) return null;
            showtime.StartTime = dto.StartTime ?? showtime.StartTime;
            showtime.EndTime = dto.EndTime ?? showtime.EndTime;
            showtime.MovieId = dto.MovieId ?? showtime.MovieId;
            showtime.RoomId = dto.RoomId ?? showtime.RoomId;
            showtime.UpdatedAt = DateTime.UtcNow;
            await _showtimeRepository.UpdateAsync(showtime);
            await _showtimeRepository.SaveChangesAsync();
            return new ShowtimeResponseDto
            {
                ShowtimeId = showtime.ShowtimeId,
                StartTime = showtime.StartTime,
                EndTime = showtime.EndTime,
                MovieId = showtime.MovieId,
                MovieName = showtime.Movie?.Title,
                RoomId = showtime.RoomId,
                RoomName = showtime.Room?.Name,
                CreatedAt = showtime.CreatedAt,
                UpdatedAt = showtime.UpdatedAt
            };
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var showtime = await _showtimeRepository.GetByIdAsync(id);
            if (showtime == null) return false;
            await _showtimeRepository.DeleteAsync(showtime);
            await _showtimeRepository.SaveChangesAsync();
            return true;
        }
        public async Task<ShowtimeResponseDto?> GetByIdAsync(int id)
        {
            var showtime = await _showtimeRepository.GetByIdAsync(id);
            if (showtime == null) return null;

            return new ShowtimeResponseDto
            {
                ShowtimeId = showtime.ShowtimeId,
                StartTime = showtime.StartTime,
                EndTime = showtime.EndTime,
                MovieId = showtime.MovieId,
                MovieName = showtime.Movie?.Title,
                RoomId = showtime.RoomId,
                RoomName = showtime.Room?.Name,
                PriceRules = showtime.PriceRules?.Select(pr => new PriceRuleResponseDto
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
                CreatedAt = showtime.CreatedAt,
                UpdatedAt = showtime.UpdatedAt
            };
        }

        public async Task<IEnumerable<ShowtimeResponseDto>> GetByRoomIdAsync(int roomId)
        {
            var showtimes = await _showtimeRepository.GetByRoomIdAsync(roomId);
            return showtimes.Select(s => new ShowtimeResponseDto
            {
                ShowtimeId = s.ShowtimeId,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                MovieId = s.MovieId,
                MovieName = s.Movie?.Title,
                RoomId = s.RoomId,
                RoomName = s.Room?.Name,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            });
        }
    }
}


