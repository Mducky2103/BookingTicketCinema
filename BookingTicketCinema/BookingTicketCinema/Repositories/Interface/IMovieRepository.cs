using BookingTicketCinema.Models;

namespace BookingTicketCinema.Repositories.Interface
{
    public interface IMovieRepository
    {
        // Lấy phim cho Carousel (ví dụ: 5 phim mới nhất)
        Task<IEnumerable<Movie>> GetFeaturedMoviesAsync();

        // LẤY PHIM DỰA TRÊN SHOWTIMES
        Task<IEnumerable<Movie>> GetNowShowingMoviesAsync(DateTime today);

        Task<IEnumerable<Movie>> GetComingSoonMoviesAsync(DateTime today);
    }
}
