using BookingTicketCinema.DTO;
using BookingTicketCinema.Models;

namespace BookingTicketCinema.Services.Interface
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieCardDto>> GetNowShowingMoviesAsync();
        Task<IEnumerable<MovieCardDto>> GetComingSoonMoviesAsync();
        Task<IEnumerable<MovieFeaturedDto>> GetFeaturedMoviesAsync();
    }
}
