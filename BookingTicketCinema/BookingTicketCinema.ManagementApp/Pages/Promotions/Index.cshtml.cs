using System.Text.Json;
using BookingTicketCinema.ManagementApp.Services;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages.Promotions
{
    public class IndexModel : PageModel
    {
        private readonly ApiClient _api;

        public List<PromotionViewModel> Promotions { get; set; } = new();

        public IndexModel(ApiClient api)
        {
            _api = api;
        }

        public async Task OnGetAsync()
        {
            try
            {
                var res = await _api.GetPromotionsAsync();
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    Promotions = JsonSerializer.Deserialize<List<PromotionViewModel>>(json, options) ?? new();
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể tải danh sách khuyến mãi.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
        }
    }
}
