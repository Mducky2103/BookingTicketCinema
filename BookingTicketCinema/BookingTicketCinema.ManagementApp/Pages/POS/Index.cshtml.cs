using System.Text.Json;
using BookingTicketCinema.ManagementApp.Services;
using BookingTicketCinema.ManagementApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.ManagementApp.Pages.POS
{
    public class IndexModel : PageModel
    {
        private readonly ApiClient _api;
        public readonly string ApiBaseUrl;

        public List<MovieViewModel> Movies { get; set; } = new();

        public IndexModel(ApiClient api, IConfiguration configuration)
        {
            _api = api;
            ApiBaseUrl = configuration["ApiBaseUrl"]!;
        }

        public async Task OnGetAsync()
        {
            var res = await _api.GetMoviesAsync();
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                Movies = JsonSerializer.Deserialize<List<MovieViewModel>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            // (Lọc phim đang chiếu nếu cần)
        }
    }
}
