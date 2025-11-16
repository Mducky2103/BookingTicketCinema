using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BookingTicketCinema.ManagementApp.Services;
using BookingTicketCinema.WebApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages.POS
{
    public class SeatMapModel : PageModel
    {
        private readonly ApiClient _api;
        public readonly string ApiBaseUrl;

        [BindProperty(SupportsGet = true)]
        public int ShowtimeId { get; set; }
        public string? ErrorMessage { get; set; }

        public ShowtimeBookingViewModel Showtime { get; set; } = new();
        public RoomViewModel Room { get; set; } = new();
        public List<SeatViewModel> Seats { get; set; } = new();
        public List<SeatGroupViewModel> SeatGroups { get; set; } = new();
        public HashSet<int> TakenSeatIds { get; set; } = new();

        public Dictionary<char, List<SeatViewModel>> SeatsByRow { get; set; } = new();

        public SeatMapModel(ApiClient api, IConfiguration config)
        {
            _api = api;
            ApiBaseUrl = config["ApiBaseUrl"]!;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (ShowtimeId == 0) return RedirectToPage("/POS/Index");

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // Lấy Suất chiếu
                var resShowtime = await _api.GetShowtimeForBookingAsync(ShowtimeId);
                if (!resShowtime.IsSuccessStatusCode) throw new Exception("Không tìm thấy suất chiếu.");
                Showtime = JsonSerializer.Deserialize<ShowtimeBookingViewModel>(
                    await resShowtime.Content.ReadAsStringAsync(), options) ?? new();

                // Lấy Phòng
                var resRoom = await _api.GetRoomByIdAsync(Showtime.RoomId);
                Room = JsonSerializer.Deserialize<RoomViewModel>(
                    await resRoom.Content.ReadAsStringAsync(), options) ?? new();

                // Lấy Ghế
                var resSeats = await _api.GetSeatsByRoomAsync(Showtime.RoomId);
                Seats = JsonSerializer.Deserialize<List<SeatViewModel>>(
                    await resSeats.Content.ReadAsStringAsync(), options) ?? new();

                // Lấy Loại ghế
                var resGroups = await _api.GetSeatGroupsByRoomAsync(Showtime.RoomId);
                SeatGroups = JsonSerializer.Deserialize<List<SeatGroupViewModel>>(
                    await resGroups.Content.ReadAsStringAsync(), options) ?? new();

                // Lấy Ghế đã bán
                var resTaken = await _api.GetTakenSeatIdsAsync(ShowtimeId);
                TakenSeatIds = JsonSerializer.Deserialize<HashSet<int>>(
                    await resTaken.Content.ReadAsStringAsync(), options) ?? new();

                ProcessSeatData();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return Page();
        }

        // Xử lý khi Staff bấm "Xác nhận Bán vé"
        public async Task<IActionResult> OnPostAsync(
            [FromForm] int showtimeId,
            [FromForm] string seatIds, // (JS sẽ gửi dạng "1,2,3")
            [FromForm] int paymentMethod) // 0=Cash, 1=CreditCard
        {
            try
            {
                var request = new
                {
                    ShowtimeId = showtimeId,
                    SeatIds = seatIds.Split(',').Select(int.Parse).ToList(),
                    Method = paymentMethod
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = await _api.BookAtCounterAsync(content);

                if (res.IsSuccessStatusCode)
                {
                    // 1. Đọc PaymentId từ kết quả trả về
                    var successJson = await res.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var paymentDto = JsonSerializer.Deserialize<PaymentResponseDto>(successJson, options);

                    // 2. Chuyển hướng sang trang Hóa đơn
                    return RedirectToPage("/POS/Receipt", new { paymentId = paymentDto.PaymentId });
                }

                var errJson = await res.Content.ReadAsStringAsync();
                var errObj = JsonSerializer.Deserialize<JsonElement>(errJson);
                ErrorMessage = errObj.TryGetProperty("message", out var msg)
                    ? msg.GetString() : "Lỗi không xác định từ API.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi hệ thống: {ex.Message}";
            }

            await OnGetAsync();
            return Page();
        }

        private void ProcessSeatData()
        {
            var groupDict = SeatGroups.ToDictionary(g => g.SeatGroupId);
            foreach (var seat in Seats)
            {
                if (groupDict.TryGetValue(seat.SeatGroupId, out var group))
                {
                    seat.SeatType = group.Type; 
                }

                if (TakenSeatIds.Contains(seat.SeatId))
                {
                    seat.Status = 1;
                }
            }

            SeatsByRow = Seats
                .Where(s => !string.IsNullOrEmpty(s.SeatNumber))
                .GroupBy(s => s.SeatNumber[0])
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.OrderBy(s =>
                    int.TryParse(s.SeatNumber.Substring(1), out int num) ? num : 99).ToList());
        }

        public class ShowtimeBookingViewModel
        {
            [JsonPropertyName("showtimeId")] public int ShowtimeId { get; set; }
            [JsonPropertyName("movieId")] public int MovieId { get; set; }
            [JsonPropertyName("movieTitle")] public string MovieTitle { get; set; } = string.Empty;
            [JsonPropertyName("roomId")] public int RoomId { get; set; }
            [JsonPropertyName("startTime")] public DateTime StartTime { get; set; }
        }
        public class RoomViewModel
        {
            [JsonPropertyName("roomId")] public int RoomId { get; set; }
            [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        }
        public class SeatViewModel
        {
            [JsonPropertyName("seatId")] public int SeatId { get; set; }
            [JsonPropertyName("seatNumber")] public string SeatNumber { get; set; } = string.Empty;
            [JsonPropertyName("seatGroupId")] public int SeatGroupId { get; set; }
            [JsonPropertyName("status")] public int Status { get; set; } // 0=Available, 1=Taken
            public int SeatType { get; set; } // 0=Thường, 1=VIP, 2=Đôi
        }
        public class SeatGroupViewModel
        {
            [JsonPropertyName("seatGroupId")] public int SeatGroupId { get; set; }
            [JsonPropertyName("groupName")] public string GroupName { get; set; } = string.Empty;
            [JsonPropertyName("type")] public int Type { get; set; }
        }
    }
}
