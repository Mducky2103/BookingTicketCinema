using System.Text.Json.Serialization;

namespace BookingTicketCinema.ManagementApp.ViewModels
{
    public class SalesReportViewModel
    {
        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("totalRevenue")]
        public decimal TotalRevenue { get; set; }

        [JsonPropertyName("totalTicketsSold")]
        public int TotalTicketsSold { get; set; }

        [JsonPropertyName("totalPayments")]
        public int TotalPayments { get; set; }
    }
}
