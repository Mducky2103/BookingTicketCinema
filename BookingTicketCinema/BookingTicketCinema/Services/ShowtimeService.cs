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


