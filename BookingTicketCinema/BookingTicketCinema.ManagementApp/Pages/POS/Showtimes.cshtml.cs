using System.Text.Json;
using System.Text.Json.Serialization;
using BookingTicketCinema.ManagementApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages.POS
{
    public class ShowtimesModel : PageModel
    {
        private readonly ApiClient _api;
        public readonly string ApiBaseUrl;

        public ShowtimesModel(ApiClient api, IConfiguration config)
        {
            _api = api;
            ApiBaseUrl = config["ApiBaseUrl"]!;
        }

        [BindProperty(SupportsGet = true)]
        public int MovieId { get; set; }

        public List<IGrouping<DateTime, ShowtimeViewModel1>> GroupedShowtimes { get; set; } = new();
        public string MovieTitle { get; set; } = "Chọn suất chiếu";
        public string? MoviePosterUrl { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (MovieId == 0) return RedirectToPage("/POS/Index");

            try
            {
                var res = await _api.GetShowtimesByMovieAsync(MovieId);
                if (!res.IsSuccessStatusCode)
                {
                    ErrorMessage = "Không thể tải suất chiếu.";
                    return Page();
                }

                var json = await res.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var showtimes = JsonSerializer.Deserialize<List<ShowtimeViewModel1>>(json, options) ?? new();

                // Lấy thông tin phim từ suất chiếu đầu tiên
                var firstShowtime = showtimes.FirstOrDefault();
                if (firstShowtime != null)
                {
                    MovieTitle = firstShowtime.MovieTitle;
                    MoviePosterUrl = firstShowtime.MoviePosterUrl;
                }

                // Lọc & Nhóm suất chiếu
                GroupedShowtimes = showtimes
                    .Where(s => s.StartTime > DateTime.Now) 
                    .OrderBy(s => s.StartTime)
                    .GroupBy(s => s.StartTime.Date) 
                    .ToList();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return Page();
        }

        public class ShowtimeViewModel1
        {
            [JsonPropertyName("showtimeId")] 
            public int Id { get; set; }
            [JsonPropertyName("startTime")]
            public DateTime StartTime { get; set; }
            [JsonPropertyName("roomName")]
            public string RoomName { get; set; } = string.Empty;
            [JsonPropertyName("movieTitle")]
            public string MovieTitle { get; set; } = string.Empty;
            [JsonPropertyName("moviePosterUrl")]
            public string? MoviePosterUrl { get; set; }
        }
    }
}
