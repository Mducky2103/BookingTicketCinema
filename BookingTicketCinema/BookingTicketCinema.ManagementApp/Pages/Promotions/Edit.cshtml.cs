using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using BookingTicketCinema.ManagementApp.Services;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages.Promotions
{
    public class EditModel : PageModel
    {
        private readonly ApiClient _api;

        public EditModel(ApiClient api)
        {
            _api = api;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string PromotionCode { get; set; } = string.Empty; // (Chỉ để hiển thị)

        // (InputModel giống Create, nhưng không có Code)
        public class InputModel
        {
            [Required(ErrorMessage = "Tỷ lệ là bắt buộc")]
            [Display(Name = "Tỷ lệ giảm giá (VD: 0.1 cho 10%)")]
            [Range(0.01, 1.0)]
            public decimal DiscountPercent { get; set; }

            [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
            [Display(Name = "Ngày bắt đầu")]
            public DateTime StartDate { get; set; }

            [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
            [Display(Name = "Ngày kết thúc")]
            public DateTime EndDate { get; set; }

            [Display(Name = "Mô tả")]
            public string? Description { get; set; }

            [Display(Name = "Kích hoạt")]
            public bool IsActive { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (Id == 0) return RedirectToPage("/Promotions/Index");

            try
            {
                var res = await _api.GetPromotionByIdAsync(Id);
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var promo = JsonSerializer.Deserialize<PromotionEditViewModel>(json, options);

                    if (promo == null) return NotFound();

                    // Map từ ViewModel (API) sang InputModel (Form)
                    PromotionCode = promo.Code; // (Lưu mã để hiển thị)
                    Input = new InputModel
                    {
                        DiscountPercent = promo.DiscountPercent,
                        StartDate = promo.StartDate.ToDateTime(TimeOnly.MinValue),
                        EndDate = promo.EndDate.ToDateTime(TimeOnly.MinValue),
                        Description = promo.Description,
                        IsActive = promo.IsActive
                    };
                    return Page();
                }
                TempData["ErrorMessage"] = "Không tìm thấy mã khuyến mãi.";
                return RedirectToPage("/Promotions/Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage("/Promotions/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Chuyển DateTime (Form) sang DateOnly (API)
            var dto = new
            {
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

                var res = await _api.UpdatePromotionAsync(Id, content);

                if (res.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật khuyến mãi thành công!";
                    return RedirectToPage("/Promotions/Index");
                }

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
