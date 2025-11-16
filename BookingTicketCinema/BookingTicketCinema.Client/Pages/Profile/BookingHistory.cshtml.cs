using BookingTicketCinema.WebApp.Services;
using BookingTicketCinema.WebApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.WebApp.Pages.Profile
{
    public class BookingHistoryModel : PageModel
    {
        private readonly IApiClientService _apiService;
        public readonly string ApiBaseUrl;
        public BookingHistoryModel(IApiClientService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            ApiBaseUrl = configuration["ApiBaseUrl"]!;
        }

        public List<TicketHistoryDto> Tickets { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var accessToken = GetToken();
                Tickets = await _apiService.GetMyTicketHistoryAsync(accessToken);
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Không tải được lịch sử: {ex.Message}";
                return RedirectToPage("/Profile/Index");
            }
        }

        // Xử lý nút "Hủy vé"
        public async Task<IActionResult> OnPostCancelAsync(int paymentId)
        {
            if (paymentId == 0) return RedirectToPage();

            try
            {
                var accessToken = GetToken();
                // Gọi API Hủy
                await _apiService.CancelPaymentAsync(paymentId, accessToken);
                TempData["SuccessMessage"] = "Đã hủy đơn hàng thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi hủy vé: {ex.Message}";
            }
            return RedirectToPage();
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
