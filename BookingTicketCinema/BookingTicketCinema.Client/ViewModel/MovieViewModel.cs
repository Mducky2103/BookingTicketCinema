using System.Text.Json.Serialization;

namespace BookingTicketCinema.WebApp.ViewModel
{
    public class MovieViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("posterUrl")]
        public string? PosterUrl { get; set; }

        [JsonPropertyName("releaseDate")]
        public DateTime ReleaseDate { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("genre")]
        public string? Genre { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
