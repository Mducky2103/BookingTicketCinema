using BookingTicketCinema.WebApp.Services;
using BookingTicketCinema.WebApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.WebApp.Pages.Booking
{
    public class SeatMapModel : PageModel
    {
        private readonly IApiClientService _apiService;

        public SeatMapModel(IApiClientService apiService)
        {
            _apiService = apiService;
        }

        // Nhận từ trang Details
        [BindProperty(SupportsGet = true)]
        public int ShowtimeId { get; set; }

        public string? ErrorMessage { get; set; }

        // === DỮ LIỆU ĐỂ HIỂN THỊ ===
        public ShowtimeBookingViewModel Showtime { get; set; } = new();
        public MovieDetailViewModel Movie { get; set; } = new();
        public RoomViewModel Room { get; set; } = new();

        // Dữ liệu ghế (đã gộp)
        public Dictionary<char, List<SeatInfo>> SeatsByRow { get; set; } = new();
        // Dữ liệu ghế đã bán
        public HashSet<int> TakenSeatIds { get; set; } = new();

        // Class nội bộ để gộp Seat và SeatGroup
        public class SeatInfo
        {
            public int SeatId { get; set; }
            public string SeatNumber { get; set; } = string.Empty;
            public int SeatType { get; set; } // 0, 1, 2
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (ShowtimeId == 0) return RedirectToPage("/Index");

            try
            {
                // 1. Lấy thông tin Suất chiếu (để lấy RoomId, MovieId)
                Showtime = await _apiService.GetShowtimeForBookingAsync(ShowtimeId);

                // 2. Lấy thông tin Phim và Phòng (Gọi song song)
                var movieTask = _apiService.GetMovieByIdAsync(Showtime.MovieId);
                var roomTask = _apiService.GetRoomByIdAsync(Showtime.RoomId);

                // 3. Lấy Sơ đồ ghế, Loại ghế, Ghế đã bán (Gọi song song)
                var seatsTask = _apiService.GetSeatsByRoomAsync(Showtime.RoomId);
                var seatGroupsTask = _apiService.GetSeatGroupsByRoomAsync(Showtime.RoomId);
                var takenSeatsTask = _apiService.GetTakenSeatIdsAsync(ShowtimeId);

                // Chờ tất cả 5 API hoàn thành
                await Task.WhenAll(movieTask, roomTask, seatsTask, seatGroupsTask, takenSeatsTask);

                Movie = movieTask.Result;
                Room = roomTask.Result;
                var allSeats = seatsTask.Result;
                var seatGroups = seatGroupsTask.Result;
                TakenSeatIds = new HashSet<int>(takenSeatsTask.Result);

                // 4. GỘP DỮ LIỆU: Map SeatGroups (Loại ghế) vào Seats (Ghế)
                var seatGroupMap = seatGroups.ToDictionary(g => g.SeatGroupId, g => g.Type);

                var processedSeats = allSeats
                    .Where(s => !string.IsNullOrEmpty(s.SeatNumber) && s.Status == 0)
                    .Select(s => new SeatInfo
                    {
                        SeatId = s.SeatId,
                        SeatNumber = s.SeatNumber,
                        // Gán loại ghế (VIP/Thường)
                        SeatType = seatGroupMap.TryGetValue(s.SeatGroupId, out var type) ? type : 0
                    });

                // 5. GỘP THEO HÀNG (A, B, C...)
                SeatsByRow = processedSeats
                    .GroupBy(s => s.SeatNumber[0])
                    .OrderBy(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.OrderBy(s => s.SeatNumber.Length).ThenBy(s => s.SeatNumber).ToList());

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi tải sơ đồ ghế: {ex.Message}";
                return Page();
            }
        }
    }
}
