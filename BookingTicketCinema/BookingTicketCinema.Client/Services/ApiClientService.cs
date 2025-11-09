using BookingTicketCinema.WebApp.ViewModel;

namespace BookingTicketCinema.WebApp.Services
{
    public class ApiClientService : IApiClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ApiClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<List<MovieFeaturedViewModel>> GetFeaturedMoviesAsync()
        {
            var client = CreateClient();
            // Gọi endpoint mới
            return await client.GetFromJsonAsync<List<MovieFeaturedViewModel>>("api/MovieForClient/featured") ?? new();
        }

        public async Task<List<MovieCardViewModel>> GetNowShowingMoviesAsync()
        {
            var client = CreateClient();
            // Gọi endpoint mới
            return await client.GetFromJsonAsync<List<MovieCardViewModel>>("api/MovieForClient/now-showing") ?? new();
        }

        public async Task<List<MovieCardViewModel>> GetComingSoonMoviesAsync()
        {
            var client = CreateClient();
            // Gọi endpoint mới
            return await client.GetFromJsonAsync<List<MovieCardViewModel>>("api/MovieForClient/coming-soon") ?? new();
        }
    }
}
