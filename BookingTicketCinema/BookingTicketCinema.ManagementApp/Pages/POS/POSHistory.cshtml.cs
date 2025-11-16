using System.Text.Json;
using BookingTicketCinema.ManagementApp.Services;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages.POS
{
    public class POSHistoryModel : PageModel
    {
        private readonly ApiClient _api;
        public readonly string ApiBaseUrl;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        public PagedResultViewModel<PaymentHistoryViewModel> PaymentHistory { get; set; } = new();

        public POSHistoryModel(ApiClient api, IConfiguration configuration)
        {
            _api = api;
            ApiBaseUrl = configuration["ApiBaseUrl"]!;
        }

        public async Task OnGetAsync()
        {
            try
            {
                var res = await _api.GetMyPOSHistoryAsync(CurrentPage, PageSize);
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    PaymentHistory = JsonSerializer.Deserialize<PagedResultViewModel<PaymentHistoryViewModel>>(json, options) ?? new();
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tải lịch sử bán vé.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
        }
    }
}
