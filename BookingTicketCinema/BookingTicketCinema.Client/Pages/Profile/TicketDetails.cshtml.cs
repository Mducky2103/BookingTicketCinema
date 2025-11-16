using BookingTicketCinema.WebApp.Services;
using BookingTicketCinema.WebApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.WebApp.Pages.Profile
{
    public class TicketDetailsModel : PageModel
    {
        private readonly IApiClientService _apiService;
        public readonly string ApiBaseUrl;

        public TicketDetailsModel(IApiClientService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            ApiBaseUrl = configuration["ApiBaseUrl"]!;
        }

        [BindProperty(SupportsGet = true)]
        public int PaymentId { get; set; }

        // Dùng lại DTO từ trang Checkout
        public PaymentResponseDto? TicketDetails { get; set; }
        public string? QrCodeUrl { get; set; } // (Link ảnh QR)

        public async Task<IActionResult> OnGetAsync()
        {
            if (PaymentId == 0)
            {
                TempData["ErrorMessage"] = "Đơn hàng không hợp lệ.";
                return RedirectToPage("/Profile/BookingHistory");
            }

            try
            {
                var accessToken = GetToken();
                TicketDetails = await _apiService.GetPaymentDetailsAsync(PaymentId, accessToken);

                // --- Tạo mã QR (Giả định) ---
                // Chúng ta sẽ dùng API public 'goqr.me' để tạo ảnh QR
                // Nội dung QR là thông tin đơn hàng (ví dụ: "CinemaTicket:123")
                string checkInData = $"CinemaTicketPaymentID:{PaymentId}";
                QrCodeUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=250x250&data={Uri.EscapeDataString(checkInData)}";

                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToPage("/Profile/BookingHistory");
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
