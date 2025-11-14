using BookingTicketCinema.WebApp.Services;
using BookingTicketCinema.WebApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.WebApp.Pages.Movie
{
    public class DetailsModel : PageModel
    {
        private readonly IApiClientService _apiService;
        public readonly string ApiBaseUrl;

        public DetailsModel(IApiClientService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            ApiBaseUrl = configuration["ApiBaseUrl"]!;
        }

        public MovieDetailViewModel? Movie { get; set; }
        public string? ErrorMessage { get; set; }

        // Dữ liệu suất chiếu đã được xử lý (gộp nhóm)
        public ILookup<DateOnly, ShowtimeDetailViewModel> ShowtimesByDate { get; set; }

        // Danh sách các ngày để hiển thị Tab (Tối đa 3 ngày)
        public List<DateOnly> AvailableDates { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                // Gọi 2 API song song
                var movieTask = _apiService.GetMovieByIdAsync(id);
                var showtimesTask = _apiService.GetShowtimesByMovieAsync(id);

                await Task.WhenAll(movieTask, showtimesTask);

                Movie = movieTask.Result;
                var showtimes = showtimesTask.Result;

                // Xử lý logic "hết suất hôm nay đẩy sang hôm sau"
                // 1. Nhóm tất cả suất chiếu theo Ngày
                ShowtimesByDate = showtimes
                                    .ToLookup(s => DateOnly.FromDateTime(s.StartTime));
                // 2. Lấy ra các ngày CÓ SUẤT CHIẾU (tối đa 3 ngày)
                AvailableDates = ShowtimesByDate
                                    .Select(g => g.Key)
                                    .OrderBy(d => d)
                                    .Take(3)
                                    .ToList();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi tải trang: {ex.Message}";
            }

            return Page();
        }
    }
}
