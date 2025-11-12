using BookingTicketCinema.ManagementApp.Services;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly IManagementApiService _apiService;
        public readonly string ApiBaseUrl;

        public IndexModel(IManagementApiService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            ApiBaseUrl = configuration["ApiBaseUrl"]!; // Lấy Base URL để hiển thị ảnh
        }

        public List<MovieViewModel> Movies { get; set; } = new();
        [TempData]
        public string? SuccessMessage { get; set; }
        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                Movies = await _apiService.GetMoviesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi tải phim: {ex.Message}";
            }
        }
    }
}
