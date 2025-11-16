using BookingTicketCinema.WebApp.Services;
using BookingTicketCinema.WebApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.WebApp.Pages.Movie
{
    public class NowShowingModel : PageModel
    {
        private readonly IApiClientService _apiService;
        public readonly string ApiBaseUrl;

        public List<MovieCardViewModel> NowShowingMovies { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public NowShowingModel(IApiClientService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            ApiBaseUrl = configuration["ApiBaseUrl"]!;
        }

        public async Task OnGetAsync()
        {
            try
            {
                NowShowingMovies = await _apiService.GetNowShowingMoviesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi tải phim: {ex.Message}";
            }
        }
    }
}
