using System.Text.Json.Serialization;

namespace BookingTicketCinema.WebApp.ViewModel
{
    public class RoomViewModel
    {
        [JsonPropertyName("roomId")]
        public int RoomId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
