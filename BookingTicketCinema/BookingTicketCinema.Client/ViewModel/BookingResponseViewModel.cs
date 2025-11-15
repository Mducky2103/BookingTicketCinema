using System.Text.Json.Serialization;

namespace BookingTicketCinema.WebApp.ViewModel
{
    public class BookingResponseViewModel
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("tickets")]
        public List<TicketResponseDTO> Tickets { get; set; } = new();
    }
}
