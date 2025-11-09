using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookingTicketCinema.WebApp.Services;
using System.Text;
using System.Text.Json;


namespace BookingTicketCinema.ManagementApp.Pages.Showtimes
{
    public class CreateModel : PageModel
    {
        private readonly IApiClientService _api;
        public CreateModel(IApiClientService api)
        {
            _api = api;
        }

        [BindProperty]
        public ShowtimeCreateDto Input { get; set; } = new();

        public class ShowtimeCreateDto
        {
            public int MovieId { get; set; }
            public int RoomId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var json = JsonSerializer.Serialize(Input);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await _api.PostAsync("/api/showtimes/CreateShowtime", content);

            if (res.IsSuccessStatusCode)
                return RedirectToPage("/Showtimes/Index");

            ModelState.AddModelError(string.Empty, "Không thể tạo suất chiếu.");
            return Page();
        }
    }
}
