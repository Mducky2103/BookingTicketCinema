using Microsoft.AspNetCore.Mvc.RazorPages;
using BookingTicketCinema.WebApp.Services;
using System.Text.Json;

namespace BookingTicketCinema.ManagementApp.Pages.Showtimes
{
    public class IndexModel : PageModel
    {
        private readonly IApiClientService _api;

        public IndexModel(IApiClientService api)
        {
            _api = api;
        }

        public List<ShowtimeViewModel> Showtimes { get; set; } = new();

        public async Task OnGetAsync()
        {
            var res = await _api.GetAsync("/api/showtimes/GetAllShowtime");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                Showtimes = JsonSerializer.Deserialize<List<ShowtimeViewModel>>(json) ?? new();
            }
        }
    }

    public class ShowtimeViewModel
    {
        public int ShowtimeId { get; set; }
        public string MovieTitle { get; set; } = "";
        public string RoomName { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
