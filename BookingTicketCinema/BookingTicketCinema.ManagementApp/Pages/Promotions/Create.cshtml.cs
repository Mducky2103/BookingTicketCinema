using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using BookingTicketCinema.ManagementApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages.Promotions
{
    public class CreateModel : PageModel
    {
        private readonly ApiClient _api;

        public CreateModel(ApiClient api)
        {
            _api = api;
        }

        // Dùng InputModel để binding dữ liệu từ Form
        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Mã code là bắt buộc")]
            [Display(Name = "Mã Code")]
            public string Code { get; set; } = string.Empty;

            [Required(ErrorMessage = "Tỷ lệ là bắt buộc")]
            [Display(Name = "Tỷ lệ giảm giá (VD: 0.1 cho 10%)")]
            [Range(0.01, 1.0, ErrorMessage = "Tỷ lệ phải từ 0.01 (1%) đến 1.0 (100%)")]
            public decimal DiscountPercent { get; set; } = 0.1m; // Mặc định 10%

            [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
            [Display(Name = "Ngày bắt đầu")]
            public DateTime StartDate { get; set; } = DateTime.Today;

            [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
            [Display(Name = "Ngày kết thúc")]
            public DateTime EndDate { get; set; } = DateTime.Today.AddDays(7);

            [Display(Name = "Mô tả")]
            public string? Description { get; set; }

            [Display(Name = "Kích hoạt")]
            public bool IsActive { get; set; } = true;
        }

        public void OnGet()
        {
            // Để Form có giá trị mặc định
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Chuyển DateTime (từ Form) sang DateOnly (cho API)
            var dto = new
            {
                Input.Code,
                Input.DiscountPercent,
                StartDate = DateOnly.FromDateTime(Input.StartDate),
                EndDate = DateOnly.FromDateTime(Input.EndDate),
                Input.Description,
                Input.IsActive
            };

            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var res = await _api.CreatePromotionAsync(content);

                if (res.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Tạo mã khuyến mãi thành công!";
                    return RedirectToPage("/Promotions/Index"); // (Tới trang Index KM)
                }

                // Xử lý lỗi từ API (VD: Mã đã tồn tại)
                var error = await res.Content.ReadAsStringAsync();
                var errObj = JsonSerializer.Deserialize<JsonElement>(error);
                ModelState.AddModelError(string.Empty, errObj.TryGetProperty("message", out var msg)
                    ? msg.GetString() : "Lỗi không xác định từ API.");
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
