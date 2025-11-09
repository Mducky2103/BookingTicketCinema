using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BookingTicketCinema.WebApp.Services;
using System.Text;
using System.Text.Json;

namespace BookingTicketCinema.ManagementApp.Pages.Showtimes
{
    public class EditModel : PageModel
    {
        private readonly IApiClientService _api;
        public EditModel(IApiClientService api)
        {
            _api = api;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public ShowtimeUpdateDto Input { get; set; } = new();

        public class ShowtimeUpdateDto
        {
            public int MovieId { get; set; }
            public int RoomId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

        public async Task OnGetAsync()
        {
            var res = await _api.GetAsync($"/api/showtimes/GetShowtimeById/{Id}");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                Input = JsonSerializer.Deserialize<ShowtimeUpdateDto>(json) ?? new();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var json = JsonSerializer.Serialize(Input);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await _api.PutAsync($"/api/showtimes/UpdateShowtime/{Id}", content);

            if (res.IsSuccessStatusCode)
                return RedirectToPage("/Showtimes/Index");

            ModelState.AddModelError(string.Empty, "Cập nhật thất bại.");
            return Page();
        }
    }
}
