using System.Text;
using System.Text.Json;
using BookingTicketCinema.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        [BindProperty(SupportsGet = false)]
        public string? SuccessMessage { get; set; }

        [BindProperty(SupportsGet = false)]
        public string? ErrorMessage { get; set; }

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
    }
}
