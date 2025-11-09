using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly CinemaDbContext _context;

        public MovieRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Movie>> GetNowShowingMoviesAsync(DateTime today)
        {
            // 1. Lấy ID của các phim có suất chiếu HÔM NAY
            var movieIds = await _context.Showtimes
                .Where(s => s.StartTime.Date == today.Date) // So sánh Date
                .Select(s => s.MovieId)
                .Distinct()
                .ToListAsync();

            // 2. Trả về thông tin các phim đó
            return await _context.Movies
                .Where(m => movieIds.Contains(m.MovieId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetComingSoonMoviesAsync(DateTime today)
        {
            // 1. Lấy ID các phim đang chiếu HÔM NAY (để loại trừ)
            var nowShowingIds = await _context.Showtimes
                .Where(s => s.StartTime.Date == today.Date)
                .Select(s => s.MovieId)
                .Distinct()
                .ToListAsync();

            // 2. Lấy ID các phim có suất chiếu TRONG TƯƠNG LAI
            var futureMovieIds = await _context.Showtimes
                .Where(s => s.StartTime.Date > today.Date)
                .Select(s => s.MovieId)
                .Distinct()
                .ToListAsync();

            // 3. Lấy ID phim SẮP CHIẾU (chỉ có trong tương lai, KHÔNG có hôm nay)
            var comingSoonIds = futureMovieIds.Except(nowShowingIds);

            return await _context.Movies
                .Where(m => comingSoonIds.Contains(m.MovieId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetFeaturedMoviesAsync()
        {
            // Logic Phim Nổi Bật: 5 phim MỚI NHẤT (theo ReleaseDate)
            return await _context.Movies
                .OrderByDescending(m => m.ReleaseDate)
                .Take(5)
                .ToListAsync();
        }
    }
}
