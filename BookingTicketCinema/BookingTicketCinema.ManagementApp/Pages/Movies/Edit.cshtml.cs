using BookingTicketCinema.ManagementApp.Services;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingTicketCinema.ManagementApp.Pages.Movies
{
    public class EditModel : PageModel
    {
        private readonly IManagementApiService _apiService;
        public readonly string ApiBaseUrl;

        public EditModel(IManagementApiService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            ApiBaseUrl = configuration["ApiBaseUrl"]!;
        }

        [BindProperty]
        public MovieEditViewModel Input { get; set; } = new();
        public SelectList StatusList { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var movie = await _apiService.GetMovieByIdAsync(id);

                // Map từ MovieViewModel (API) sang MovieEditViewModel (Form)
                Input = new MovieEditViewModel
                {
                    MovieId = movie.MovieId,
                    Title = movie.Title,
                    Description = movie.Description,
                    Genre = movie.Genre,
                    TrailerUrl = movie.TrailerUrl,
                    Duration = movie.Duration.ToString(@"hh\:mm\:ss"), // Chuyển TimeSpan -> string
                    ReleaseDate = movie.ReleaseDate.ToString("yyyy-MM-dd"), // Chuyển DateOnly -> string
                    Status = movie.Status,
                    CurrentPosterUrl = movie.PosterUrl,
                    CurrentBackdropUrl = movie.BackdropUrl
                };

                LoadStatusList();
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
            LoadStatusList();
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _apiService.UpdateMovieAsync(Input.MovieId, Input);
                TempData["SuccessMessage"] = $"Cập nhật phim '{Input.Title}' thành công!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi cập nhật: {ex.Message}");
                return Page();
            }
        }

        private void LoadStatusList()
        {
            var statuses = new[]
            {
                new { Id = 0, Name = "Sắp chiếu" },
                new { Id = 1, Name = "Đang chiếu" },
                new { Id = 2, Name = "Đã kết thúc" }
            };
            StatusList = new SelectList(statuses, "Id", "Name", Input.Status);
        }
    }
}
