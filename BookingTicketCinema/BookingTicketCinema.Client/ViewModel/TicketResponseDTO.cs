using System.Text.Json.Serialization;

namespace BookingTicketCinema.WebApp.ViewModel
{
    public class TicketResponseDTO
    {
        [JsonPropertyName("ticketId")]
        public int TicketId { get; set; }
    }
}
