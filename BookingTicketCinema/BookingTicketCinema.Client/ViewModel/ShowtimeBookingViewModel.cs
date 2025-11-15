using System.Text.Json.Serialization;

namespace BookingTicketCinema.WebApp.ViewModel
{
    public class ShowtimeBookingViewModel
    {
        [JsonPropertyName("showtimeId")]
        public int ShowtimeId { get; set; }
        [JsonPropertyName("movieId")]
        public int MovieId { get; set; }
        [JsonPropertyName("roomId")]
        public int RoomId { get; set; }
        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; }
    }
}
