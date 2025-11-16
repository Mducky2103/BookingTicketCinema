using System.Text.Json;
using BookingTicketCinema.ManagementApp.Services;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages.Reports
{
    public class SalesModel : PageModel
    {
        private readonly ApiClient _api;

        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; } = DateTime.Today;

        // Kết quả báo cáo
        public SalesReportViewModel? Report { get; set; }
        public string? ErrorMessage { get; set; }

        public SalesModel(ApiClient api)
        {
            _api = api;
        }

        public async Task OnGetAsync()
        {
            // Trang này sẽ luôn tải báo cáo dựa trên ngày trong URL
            // (Mặc định là Hôm nay)
            try
            {
                var res = await _api.GetSalesReportAsync(StartDate, EndDate);
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    Report = JsonSerializer.Deserialize<SalesReportViewModel>(json, options);
                }
                else
                {
                    ErrorMessage = "Không thể tải báo cáo từ API.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
