using BookingTicketCinema.Models;

namespace BookingTicketCinema.Repositories.Interface
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetFeaturedMoviesAsync();

        Task<IEnumerable<Movie>> GetNowShowingMoviesAsync(DateTime today);

        Task<IEnumerable<Movie>> GetComingSoonMoviesAsync(DateTime today);
        Task<IEnumerable<Movie>> GetMoviesAsync(string? searchTerm = null);
    }
}
