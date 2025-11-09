using BookingTicketCinema.WebApp.ViewModel;

namespace BookingTicketCinema.WebApp.Services
{
    public interface IApiClientService
    {
        Task<List<MovieFeaturedViewModel>> GetFeaturedMoviesAsync();
        Task<List<MovieCardViewModel>> GetNowShowingMoviesAsync();
        Task<List<MovieCardViewModel>> GetComingSoonMoviesAsync();
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> PostAsync(string url, HttpContent content);
        Task<HttpResponseMessage> PutAsync(string url, HttpContent content);
        Task<HttpResponseMessage> DeleteAsync(string url);

    }
}
