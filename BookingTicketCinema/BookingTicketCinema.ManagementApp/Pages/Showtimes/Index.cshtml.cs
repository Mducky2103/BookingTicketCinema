using System.Text;
using System.Text.Json;
using BookingTicketCinema.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingTicketCinema.ManagementApp.Pages.Showtimes
{
    public class IndexModel : PageModel
    {
        private readonly IApiClientService _api;
        public IndexModel(IApiClientService api)
        {
            _api = api;
        }

        // =========================
        // PROPERTIES
        // =========================
        public List<ShowtimeViewModel> Showtimes { get; set; } = new();
        public List<SelectListItem> MovieOptions { get; set; } = new();
        public List<SelectListItem> RoomOptions { get; set; } = new();

        [BindProperty(SupportsGet = false)]
        public string? SuccessMessage { get; set; }

        [BindProperty(SupportsGet = false)]
        public string? ErrorMessage { get; set; }

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

            var res = await _api.GetAsync("/api/showtimes/GetAllShowtime");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                Showtimes = JsonSerializer.Deserialize<List<ShowtimeViewModel>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            else
            {
                var err = await res.Content.ReadAsStringAsync();
                TempData["Error"] = $"Không tải được danh sách suất chiếu: {err}";
            }
        }

        // =========================
        // TẢI DANH SÁCH PHIM & PHÒNG
        // =========================
        private async Task LoadDropdownDataAsync()
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

            // fallback nếu API rỗng
            if (MovieOptions.Count == 0)
                MovieOptions.Add(new SelectListItem { Value = "", Text = "Không có phim khả dụng" });
            if (RoomOptions.Count == 0)
                RoomOptions.Add(new SelectListItem { Value = "", Text = "Không có phòng khả dụng" });
        }

        // =========================
        // THÊM MỚI
        // =========================
        public async Task<IActionResult> OnPostCreateAsync(ShowtimeInput input)
        {
            var json = JsonSerializer.Serialize(input);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await _api.PostAsync("/api/showtimes/CreateShowtime", content);

            if (res.IsSuccessStatusCode)
            {
                TempData["Success"] = "Đã thêm suất chiếu mới!";
            }
            else
            {
                var err = await res.Content.ReadAsStringAsync();
                TempData["Error"] = $"Thêm thất bại: {err}";
            }

            return RedirectToPage();
        }

        // =========================
        // CẬP NHẬT
        // =========================
        public async Task<IActionResult> OnPostUpdateAsync(ShowtimeInput input)
        {
            var json = JsonSerializer.Serialize(input);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await _api.PutAsync($"/api/showtimes/UpdateShowtime/{input.ShowtimeId}", content);

            if (res.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật thành công!";
            }
            else
            {
                var err = await res.Content.ReadAsStringAsync();
                TempData["Error"] = $"Cập nhật thất bại: {err}";
            }

            return RedirectToPage();
        }

        // =========================
        // XÓA
        // =========================
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var res = await _api.DeleteAsync($"/api/showtimes/DeleteShowtime/{id}");

            if (res.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa thành công!";
            }
            else
            {
                var err = await res.Content.ReadAsStringAsync();
                TempData["Error"] = $"Xóa thất bại: {err}";
            }

            return RedirectToPage();
        }

        // =========================
        // VIEW MODEL
        // =========================
        public class ShowtimeViewModel
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
        // PHỤ TRỢ CHO DROPDOWN
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
