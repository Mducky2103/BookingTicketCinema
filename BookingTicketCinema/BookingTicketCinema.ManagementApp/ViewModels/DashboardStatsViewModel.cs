using System.Text.Json.Serialization;

namespace BookingTicketCinema.ManagementApp.ViewModels
{
    public class DashboardStatsViewModel
    {
        [JsonPropertyName("todayRevenue")]
        public decimal TodayRevenue { get; set; }

        [JsonPropertyName("ticketsSoldToday")]
        public int TicketsSoldToday { get; set; }

        [JsonPropertyName("totalCustomers")]
        public int TotalCustomers { get; set; }

        [JsonPropertyName("totalMovies")]
        public int TotalMovies { get; set; }
    }
}
