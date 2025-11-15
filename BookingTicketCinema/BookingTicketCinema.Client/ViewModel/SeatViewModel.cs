using System.Text.Json.Serialization;

namespace BookingTicketCinema.WebApp.ViewModel
{
    public class SeatViewModel
    {
        [JsonPropertyName("seatId")]
        public int SeatId { get; set; }
        [JsonPropertyName("seatNumber")]
        public string SeatNumber { get; set; } = string.Empty;
        [JsonPropertyName("seatGroupId")]
        public int SeatGroupId { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}
