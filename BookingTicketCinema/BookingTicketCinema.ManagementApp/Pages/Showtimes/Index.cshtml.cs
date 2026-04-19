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

        // Paging
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

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
            //public DateTime EndTime { get; set; }
        }

        // =========================
        // HIỂN THỊ DANH SÁCH
        // =========================
        public async Task OnGetAsync(int pageNumber = 1, int pageSize = 15)
        {
            CurrentPage = pageNumber <= 0 ? 1 : pageNumber;
            PageSize = pageSize <= 0 ? 15 : pageSize;

            await LoadDropdownDataAsync();

            try
            {
                var url = $"/api/showtimes/GetAllShowtime?pageNumber={CurrentPage}&pageSize={PageSize}";
                var res = await _api.GetAsync(url);

                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();

                    // Try to parse paged result first; fall back to plain list for backward compatibility
                    if (!string.IsNullOrWhiteSpace(json) && json.TrimStart().StartsWith("["))
                    {
                        // old format: list
                        Showtimes = JsonSerializer.Deserialize<List<ShowtimeViewModel1>>(json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                        TotalCount = Showtimes.Count;
                        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                    }
                    else
                    {
                        var paged = JsonSerializer.Deserialize<PagedResult<ShowtimeViewModel1>>(json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                                   ?? new PagedResult<ShowtimeViewModel1>();

                        Showtimes = paged.Items ?? new List<ShowtimeViewModel1>();
                        CurrentPage = paged.PageNumber <= 0 ? CurrentPage : paged.PageNumber;
                        PageSize = paged.PageSize <= 0 ? PageSize : paged.PageSize;
                        TotalCount = paged.TotalCount;
                        TotalPages = paged.TotalPages;
                    }
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
                    Success = "Tạo suất chiếu thành công";
                else
                    Error = ExtractApiError(await res.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            return RedirectToPage(new { pageNumber = CurrentPage, pageSize = PageSize });
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
                    Success = "Cập nhật thành công";
                else
                    Error = ExtractApiError(await res.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            return RedirectToPage(new { pageNumber = CurrentPage, pageSize = PageSize });
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
                    Success = "Xóa thành công";
                else
                    Error = ExtractApiError(await res.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }

            return RedirectToPage(new { pageNumber = CurrentPage, pageSize = PageSize });
        }

        // =========================
        // CREATE BULK
        // =========================
        public async Task<IActionResult> OnPostCreateBulkAsync(int MovieId, int RoomId, string StartTimesText)
        {
            try
            {
                var times = StartTimesText
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => DateTime.Parse(t.Trim()))
                    .ToList();

                var payload = new
                {
                    MovieId,
                    RoomId,
                    StartTimes = times
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = await _api.PostAsync("/api/showtimes/CreateBulkShowtime", content);

                if (res.IsSuccessStatusCode)
                {
                    var response = await res.Content.ReadAsStringAsync();
                    Success = "Tạo hàng loạt thành công!";
                }
                else
                {
                    var err = await res.Content.ReadAsStringAsync();
                    Error = ExtractApiError(err);
                }
            }
            catch (Exception ex)
            {
                Error = $"Lỗi bulk: {ex.Message}";
            }

            return RedirectToPage(new { pageNumber = CurrentPage, pageSize = PageSize });
        }

        // =========================
        // HÀM TIỆN ÍCH: LỌC LỖI API
        // =========================
        private static string ExtractApiError(string rawError)
        {
            if (string.IsNullOrWhiteSpace(rawError))
                return "Không rõ lỗi";

            if (rawError.Contains(" at "))
                rawError = rawError.Split(" at ")[0];

            if (rawError.Contains(":"))
                rawError = rawError.Split(':').Last().Trim();

            return rawError.Replace("\n", "").Replace("\r", "").Trim();
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

        // Local PagedResult to match API response without adding a shared DTO reference
        private class PagedResult<T>
        {
            public List<T> Items { get; set; } = new();
            public int TotalCount { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
        }
    }
}
