using System.Text.Json.Serialization;

namespace BookingTicketCinema.WebApp.ViewModel
{
    public class MovieDetailViewModel
    {
        [JsonPropertyName("movieId")]
        public int MovieId { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("posterUrl")]
        public string? PosterUrl { get; set; }
        [JsonPropertyName("trailerUrl")]
        public string? TrailerUrl { get; set; }
        [JsonPropertyName("genre")]
        public string? Genre { get; set; }
        [JsonPropertyName("duration")]
        public TimeSpan Duration { get; set; }
        [JsonPropertyName("releaseDate")]
        public DateOnly ReleaseDate { get; set; }
    }
}
