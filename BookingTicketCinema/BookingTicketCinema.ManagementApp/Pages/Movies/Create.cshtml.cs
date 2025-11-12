using BookingTicketCinema.ManagementApp.Services;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingTicketCinema.ManagementApp.Pages.Movies
{
    public class CreateModel : PageModel
    {
        private readonly IManagementApiService _apiService;

        public CreateModel(IManagementApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty]
        public MovieCreateViewModel Input { get; set; } = new();
        public SelectList StatusList { get; set; }

        public void OnGet()
        {
            LoadStatusList();
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
                await _apiService.CreateMovieAsync(Input);
                TempData["SuccessMessage"] = $"Tạo phim '{Input.Title}' thành công!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi tạo phim: {ex.Message}");
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
