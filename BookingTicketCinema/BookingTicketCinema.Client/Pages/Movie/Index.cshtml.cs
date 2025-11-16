using BookingTicketCinema.WebApp.Services;
using BookingTicketCinema.WebApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.WebApp.Pages.Movie
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

        public List<MovieViewModel> Movies { get; set; } = new();

        // Nhận giá trị `search` từ form (GET)
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Gọi API với search term (nếu có)
                Movies = await _apiService.GetMoviesAsync(SearchTerm);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi tải phim: {ex.Message}";
            }
        }
    }
}
