using BookingTicketCinema.WebApp.ViewModel;

namespace BookingTicketCinema.WebApp.Services
{
    public interface IApiClientService
    {
        Task<List<MovieFeaturedViewModel>> GetFeaturedMoviesAsync();
        Task<List<MovieCardViewModel>> GetNowShowingMoviesAsync();
        Task<List<MovieCardViewModel>> GetComingSoonMoviesAsync();
    }
}
