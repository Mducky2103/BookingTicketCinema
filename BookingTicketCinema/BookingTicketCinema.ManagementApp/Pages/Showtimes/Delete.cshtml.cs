using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookingTicketCinema.WebApp.Services;

namespace BookingTicketCinema.ManagementApp.Pages.Showtimes
{
    public class DeleteModel : PageModel
    {
        private readonly IApiClientService _api;
        public DeleteModel(IApiClientService api)
        {
            _api = api;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var res = await _api.DeleteAsync($"/api/showtimes/DeleteShowtime/{Id}");
            if (res.IsSuccessStatusCode)
                return RedirectToPage("/Showtimes/Index");

            ModelState.AddModelError(string.Empty, "Không thể xóa suất chiếu.");
            return Page();
        }
    }
}
