using BookingTicketCinema.WebApp.Services;
using BookingTicketCinema.WebApp.ViewModel;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.WebApp.Pages.Booking
{
    public class CheckoutModel : PageModel
    {
        private readonly IApiClientService _apiService;
        private readonly IAntiforgery _antiforgery; 
        public string RequestVerificationToken { get; private set; } 

        // Dùng để lưu MovieId tạm thời khi redirect
        [TempData]
        public int? LastMovieId { get; set; }

        public CheckoutModel(IApiClientService apiService, IAntiforgery antiforgery) 
        {
            _apiService = apiService;
            _antiforgery = antiforgery; 
        }

        [BindProperty(SupportsGet = true)]
        public int? ShowtimeId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? SeatIds { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PaymentIdFromHistory { get; set; }

        [BindProperty]
        public int PaymentId { get; set; }

        public PaymentResponseDto? PaymentSummary { get; set; }
        public string? ErrorMessage { get; set; }
        public class VoucherApplyInput
        {
            public int PaymentId { get; set; }
            public string Code { get; set; } = string.Empty;
        }
        public async Task<IActionResult> OnPostApplyVoucherAsync([FromBody] VoucherApplyInput input)
        {
            // (Xác thực Token thủ công cho AJAX)
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);
            }
            catch (AntiforgeryValidationException)
            {
                return new BadRequestObjectResult(new { message = "Lỗi bảo mật." });
            }

            try
            {
                var accessToken = GetToken();
                // Gọi Service đã viết
                var updatedPayment = await _apiService.ApplyVoucherAsync(input.PaymentId, input.Code, accessToken);

                // Trả về DTO mới (chỉ chứa giá tiền)
                return new OkObjectResult(new { amount = updatedPayment.Amount });
            }
            catch (Exception ex)
            {
                // Trả về lỗi (VD: "Mã đã dùng")
                return new BadRequestObjectResult(new { message = ex.Message });
            }
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var accessToken = GetToken();

                if (PaymentIdFromHistory.HasValue)
                {
                    PaymentSummary = await _apiService.GetPaymentSummaryAsync(PaymentIdFromHistory.Value, accessToken);
                }
                else if (ShowtimeId.HasValue && !string.IsNullOrEmpty(SeatIds))
                {
                    var request = new PaymentRequestDto
                    {
                        ShowtimeId = this.ShowtimeId.Value,
                        SeatIds = this.SeatIds.Split(',').Select(int.Parse).ToList()
                    };

                    PaymentSummary = await _apiService.CreatePendingPaymentAsync(request, accessToken);

                    var showtime = await _apiService.GetShowtimeForBookingAsync(ShowtimeId.Value);
                    LastMovieId = showtime.MovieId;

                    return RedirectToPage(new { PaymentIdFromHistory = PaymentSummary.PaymentId });
                }
                else
                {
                    TempData["ErrorMessage"] = "Yêu cầu không hợp lệ.";
                    return RedirectToPage("/Index");
                }

                this.PaymentId = PaymentSummary.PaymentId;
                RequestVerificationToken = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                if (PaymentIdFromHistory.HasValue)
                    return RedirectToPage("/Profile/BookingHistory");
                else
                    return RedirectToPage("/Movie/Details", new { id = LastMovieId ?? 1 });
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (PaymentId == 0)
            {
                ErrorMessage = "Đơn hàng không hợp lệ, vui lòng thử lại.";
                return Page();
            }
            try
            {
                var accessToken = GetToken();
                await _apiService.ConfirmPaymentAsync(PaymentId, accessToken);
                TempData["SuccessMessage"] = "Thanh toán thành công!";
                return RedirectToPage("/Profile/BookingHistory");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Thanh toán thất bại: {ex.Message}";
                PaymentSummary = await _apiService.GetPaymentSummaryAsync(this.PaymentId, GetToken());
                return Page();
            }
        }

        public async Task<IActionResult> OnPostCancelAsync()
        {
            if (PaymentId == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng để hủy.";
                return RedirectToPage("/Index");
            }
            try
            {
                var accessToken = GetToken();
                await _apiService.CancelPaymentAsync(PaymentId, accessToken);
                TempData["SuccessMessage"] = "Đã hủy đơn hàng thành công.";

                int.TryParse(Request.Form["ShowtimeId"], out int showtimeId);
                if (showtimeId > 0)
                {
                    return RedirectToPage("/Movie/Details", new { id = LastMovieId ?? 1 });
                }
                return RedirectToPage("/Profile/BookingHistory");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi hủy: {ex.Message}";
                return RedirectToPage(new { PaymentIdFromHistory = this.PaymentId });
            }
        }

        private string GetToken()
        {
            var accessToken = User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
            if (string.IsNullOrEmpty(accessToken))
                throw new Exception("Phiên đăng nhập hết hạn.");
            return accessToken;
        }
    }
}
