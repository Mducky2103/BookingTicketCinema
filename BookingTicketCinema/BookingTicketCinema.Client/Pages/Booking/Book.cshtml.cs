using BookingTicketCinema.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace BookingTicketCinema.Client.Pages.Booking
{
    public class BookModel : PageModel
    {
        private readonly IApiClientService _api;

        public BookModel(IApiClientService api)
        {
            _api = api;
        }

        [BindProperty(SupportsGet = true)]
        public int ShowtimeId { get; set; }

        public string MovieTitle { get; set; } = "";
        public string ShowtimeInfo { get; set; } = "";
        public string UserId { get; set; } = "user001";
        public List<SeatViewModel> Seats { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Gọi API lấy danh sách ghế theo suất chiếu
            var response = await _api.GetAsync($"/api/seats/showtime/{ShowtimeId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Seats = JsonSerializer.Deserialize<List<SeatViewModel>>(json) ?? new();
            }
        }

        public async Task<IActionResult> OnPostBookAsync([FromBody] BookingRequestDTO request)
        {
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var res = await _api.PostAsync("/api/booking/book", content);
            var result = await res.Content.ReadAsStringAsync();
            return Content(result, "application/json");
        }
    }

    public class BookingRequestDTO
    {
        public int ShowtimeId { get; set; }
        public List<int> SeatIds { get; set; } = new();
        public string UserId { get; set; } = string.Empty;
    }

    public class SeatViewModel
    {
        public int SeatId { get; set; }
        public string SeatLabel { get; set; } = "";
        public bool IsReserved { get; set; }
    }
}
