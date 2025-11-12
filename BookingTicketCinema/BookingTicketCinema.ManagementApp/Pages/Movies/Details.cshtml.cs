using BookingTicketCinema.ManagementApp.Services;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages.Movies
{
    public class DetailsModel : PageModel
    {
        private readonly IManagementApiService _apiService;
        public readonly string ApiBaseUrl;

        public DetailsModel(IManagementApiService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            ApiBaseUrl = configuration["ApiBaseUrl"]!;
        }

        public MovieViewModel? Movie { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Movie = await _apiService.GetMovieByIdAsync(id);
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi tải chi tiết: {ex.Message}";
                return Page();
            }
        }
    }
}
