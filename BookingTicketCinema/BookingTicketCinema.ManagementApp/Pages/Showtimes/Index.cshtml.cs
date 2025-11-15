using System.Text;
using System.Text.Json;
using BookingTicketCinema.ManagementApp.Services;
using BookingTicketCinema.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingTicketCinema.ManagementApp.Pages.Showtimes
{
    public class IndexModel : PageModel
    {
        private readonly ApiClient _api;
        public IndexModel(ApiClient api)
        {
            _api = api;
        }

        // =========================
        // PROPERTIES
        // =========================
        public List<ShowtimeViewModel1> Showtimes { get; set; } = new();
        public List<SelectListItem> MovieOptions { get; set; } = new();
        public List<SelectListItem> RoomOptions { get; set; } = new();

        [TempData] public string? Success { get; set; }
        [TempData] public string? Error { get; set; }

        // =========================
        // DTO INPUT
        // =========================
        public class ShowtimeInput
        {
            public int ShowtimeId { get; set; }
            public int MovieId { get; set; }
            public int RoomId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

        // =========================
        // HIỂN THỊ DANH SÁCH
        // =========================
        public async Task OnGetAsync()
        {
            await LoadDropdownDataAsync();

            try
            {
                var res = await _api.GetAsync("/api/showtimes/GetAllShowtime");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    Showtimes = JsonSerializer.Deserialize<List<ShowtimeViewModel1>>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                }
                else
                {
                    var err = await res.Content.ReadAsStringAsync();
                    Error = $"Không tải được danh sách suất chiếu: {ExtractApiError(err)}";
                }
            }
            catch (Exception ex)
            {
                Error = $"Lỗi khi tải danh sách: {ex.Message}";
            }
        }

        // =========================
        // TẢI DANH SÁCH PHIM & PHÒNG
        // =========================
        private async Task LoadDropdownDataAsync()
        {
            try
            {
                // --- Movies ---
                var resMovie = await _api.GetAsync("/api/Movie");
                if (resMovie.IsSuccessStatusCode)
                {
                    var json = await resMovie.Content.ReadAsStringAsync();
                    var movies = JsonSerializer.Deserialize<List<MovieDto>>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                    MovieOptions = movies.Select(m => new SelectListItem
                    {
                        Value = m.MovieId.ToString(),
                        Text = m.Title
                    }).ToList();
                }

                // --- Rooms ---
                var resRoom = await _api.GetAsync("/api/rooms");
                if (resRoom.IsSuccessStatusCode)
                {
                    var json = await resRoom.Content.ReadAsStringAsync();
                    var rooms = JsonSerializer.Deserialize<List<RoomDto>>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                    RoomOptions = rooms.Select(r => new SelectListItem
                    {
                        Value = r.RoomId.ToString(),
                        Text = $"{r.Name} ({(r.Type == 0 ? "2D" : r.Type == 1 ? "3D" : "IMAX")})"
                    }).ToList();
                }

                if (MovieOptions.Count == 0)
                    MovieOptions.Add(new SelectListItem { Value = "", Text = "Không có phim khả dụng" });
                if (RoomOptions.Count == 0)
                    RoomOptions.Add(new SelectListItem { Value = "", Text = "Không có phòng khả dụng" });
            }
            catch (Exception ex)
            {
                Error = $"Không tải được dữ liệu dropdown: {ex.Message}";
            }
        }

        // =========================
        // THÊM MỚI
        // =========================
        public async Task<IActionResult> OnPostCreateAsync(ShowtimeInput input)
        {
            try
            {
                var json = JsonSerializer.Serialize(input);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await _api.PostAsync("/api/showtimes/CreateShowtime", content);

                if (res.IsSuccessStatusCode)
                {
                    Success = "Đã thêm suất chiếu mới thành công.";
                }
                else
                {
                    var err = await res.Content.ReadAsStringAsync();
                    Error = $"Thêm thất bại: {ExtractApiError(err)}";
                }
            }
            catch (Exception ex)
            {
                Error = $"Lỗi hệ thống khi thêm: {ex.Message}";
            }

            return RedirectToPage();
        }

        // =========================
        // CẬP NHẬT
        // =========================
        public async Task<IActionResult> OnPostUpdateAsync(ShowtimeInput input)
        {
            try
            {
                var json = JsonSerializer.Serialize(input);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await _api.PutAsync($"/api/showtimes/UpdateShowtime/{input.ShowtimeId}", content);

                if (res.IsSuccessStatusCode)
                {
                    Success = "Cập nhật suất chiếu thành công.";
                }
                else
                {
                    var err = await res.Content.ReadAsStringAsync();
                    Error = $"Cập nhật thất bại: {ExtractApiError(err)}";
                }
            }
            catch (Exception ex)
            {
                Error = $"Lỗi hệ thống khi cập nhật: {ex.Message}";
            }

            return RedirectToPage();
        }

        // =========================
        // XÓA
        // =========================
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var res = await _api.DeleteAsync($"/api/showtimes/DeleteShowtime/{id}");

                if (res.IsSuccessStatusCode)
                {
                    Success = "Đã xóa suất chiếu thành công.";
                }
                else
                {
                    var err = await res.Content.ReadAsStringAsync();
                    Error = $"Xóa thất bại: {ExtractApiError(err)}";
                }
            }
            catch (Exception ex)
            {
                Error = $"Lỗi hệ thống khi xóa: {ex.Message}";
            }

            return RedirectToPage();
        }

        // =========================
        // HÀM TIỆN ÍCH: LỌC LỖI API
        // =========================
        private static string ExtractApiError(string rawError)
        {
            if (string.IsNullOrWhiteSpace(rawError))
                return "Không rõ nguyên nhân.";

            string msg = rawError;

            // Cắt phần stack trace
            if (msg.Contains(" at "))
                msg = msg.Split(" at ")[0];

            // Lấy phần sau dấu ":" cuối cùng (thường là thông điệp lỗi chính)
            if (msg.Contains(":"))
                msg = msg.Split(':').LastOrDefault()?.Trim() ?? msg;

            // Làm sạch ký tự đặc biệt và dòng thừa
            msg = msg.Replace("\r", "").Replace("\n", "").Trim();

            return msg;
        }

        // =========================
        // VIEW MODEL
        // =========================
        public class ShowtimeViewModel1
        {
            public int ShowtimeId { get; set; }
            public int MovieId { get; set; }
            public string MovieName { get; set; } = string.Empty;
            public int RoomId { get; set; }
            public string RoomName { get; set; } = string.Empty;
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }

        // =========================
        // DTO CHO DROPDOWN
        // =========================
        private class MovieDto
        {
            public int MovieId { get; set; }
            public string Title { get; set; } = string.Empty;
        }

        private class RoomDto
        {
            public int RoomId { get; set; }
            public string Name { get; set; } = string.Empty;
            public byte Type { get; set; }
        }
    }
}
