using BookingTicketCinema.ManagementApp.Services;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages.Movies
{
    public class DeleteModel : PageModel
    {
        private readonly IManagementApiService _apiService;
        public readonly string ApiBaseUrl;

        public DeleteModel(IManagementApiService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            ApiBaseUrl = configuration["ApiBaseUrl"]!;
        }

        [BindProperty]
        public MovieViewModel Movie { get; set; } = new();
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
                TempData["ErrorMessage"] = $"Lỗi tải phim: {ex.Message}";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await _apiService.DeleteMovieAsync(Movie.MovieId);
                TempData["SuccessMessage"] = $"Đã xóa phim '{Movie.Title}' thành công.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi khi xóa phim: {ex.Message}";
                return Page();
            }
        }
    }
}
