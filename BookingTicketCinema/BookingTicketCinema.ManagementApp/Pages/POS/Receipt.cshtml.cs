using System.Text.Json;
using System.Text.Json.Serialization;
using BookingTicketCinema.ManagementApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages.POS
{
    public class ReceiptModel : PageModel
    {
        private readonly ApiClient _api;

        [BindProperty(SupportsGet = true)]
        public int PaymentId { get; set; }

        public PaymentResponseDto1? Receipt { get; set; }
        public string? ErrorMessage { get; set; }

        public ReceiptModel(ApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (PaymentId == 0) return RedirectToPage("/POS/Index");

            try
            {
                var res = await _api.GetReceiptAsync(PaymentId);
                if (!res.IsSuccessStatusCode)
                {
                    ErrorMessage = "Không thể tải thông tin hóa đơn.";
                    return Page();
                }

                var json = await res.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Receipt = JsonSerializer.Deserialize<PaymentResponseDto1>(json, options);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return Page();
        }

        public class PaymentResponseDto1
        {
            [JsonPropertyName("paymentId")] public int PaymentId { get; set; }
            [JsonPropertyName("amount")] public decimal Amount { get; set; }
            [JsonPropertyName("status")] public int Status { get; set; }
            [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; }
            [JsonPropertyName("movieTitle")] public string MovieTitle { get; set; } = string.Empty;
            [JsonPropertyName("roomName")] public string RoomName { get; set; } = string.Empty;
            [JsonPropertyName("showtime")] public DateTime Showtime { get; set; }
            [JsonPropertyName("seatNumbers")] public List<string> SeatNumbers { get; set; } = new();
        }
    }
}
