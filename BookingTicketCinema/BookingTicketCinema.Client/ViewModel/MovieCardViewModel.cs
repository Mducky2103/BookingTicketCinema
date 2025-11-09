using System.Text.Json.Serialization;

namespace BookingTicketCinema.WebApp.ViewModel
{
    public class MovieCardViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("posterUrl")]
        public string PosterUrl { get; set; } = string.Empty;
        [JsonPropertyName("duration")]
        public int Duration { get; set; }
        [JsonPropertyName("genre")]
        public string Genre { get; set; } = string.Empty;
    }
}
