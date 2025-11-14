using BookingTicketCinema.WebApp.Services;
using BookingTicketCinema.WebApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.Client.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IApiClientService _apiService;
        public readonly string ApiBaseUrl;
        public IndexModel(IApiClientService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            ApiBaseUrl = configuration["ApiBaseUrl"]!;
        }

        public List<MovieFeaturedViewModel> FeaturedMovies { get; set; } = new();
        public List<MovieCardViewModel> NowShowingMovies { get; set; } = new();
        public List<MovieCardViewModel> ComingSoonMovies { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                var featuredTask = _apiService.GetFeaturedMoviesAsync();
                var nowShowingTask = _apiService.GetNowShowingMoviesAsync();
                var comingSoonTask = _apiService.GetComingSoonMoviesAsync();

                await Task.WhenAll(featuredTask, nowShowingTask, comingSoonTask);

                FeaturedMovies = featuredTask.Result;
                NowShowingMovies = nowShowingTask.Result;
                ComingSoonMovies = comingSoonTask.Result;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Không thể tải dữ liệu phim: {ex.Message}";
            }
        }
    }
}
