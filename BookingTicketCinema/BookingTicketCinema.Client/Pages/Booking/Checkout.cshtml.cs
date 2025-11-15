using BookingTicketCinema.WebApp.Services;
using BookingTicketCinema.WebApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.WebApp.Pages.Booking
{
    public class CheckoutModel : PageModel
    {
        private readonly IApiClientService _apiService;

        public CheckoutModel(IApiClientService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty(SupportsGet = true)]
        public int ShowtimeId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SeatIds { get; set; } = string.Empty;

        [BindProperty]
        public int PaymentId { get; set; }

        public PaymentResponseDto? PaymentSummary { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (ShowtimeId == 0 || string.IsNullOrEmpty(SeatIds))
            {
                return RedirectToPage("/Index");
            }

            try
            {
                var accessToken = GetToken();
                var request = new PaymentRequestDto
                {
                    ShowtimeId = this.ShowtimeId,
                    SeatIds = this.SeatIds.Split(',').Select(int.Parse).ToList()
                };

                // Gọi API để tạo đơn hàng "Pending" VÀ lấy tổng tiền
                PaymentSummary = await _apiService.CreatePendingPaymentAsync(request, accessToken);

                // Lưu PaymentId để dùng cho OnPost
                PaymentId = PaymentSummary.PaymentId;

                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Không thể đặt vé: {ex.Message}";
                return RedirectToPage("/Movie/Details", new { id = TempData["LastMovieId"] ?? 1 });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (PaymentId == 0)
            {
                ErrorMessage = "Đơn hàng không hợp lệ.";
                await OnGetAsync(); 
                return Page();
            }

            try
            {
                var accessToken = GetToken();

                await _apiService.ConfirmPaymentAsync(PaymentId, accessToken);

                TempData["SuccessMessage"] = "Thanh toán thành công! Vé của bạn đã được xác nhận.";
                return RedirectToPage("/Profile/BookingHistory");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Thanh toán thất bại: {ex.Message}";
                await OnGetAsync(); 
                return Page();
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
