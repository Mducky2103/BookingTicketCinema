using System.Text.Json.Serialization;

namespace BookingTicketCinema.WebApp.ViewModel
{
    public class MovieFeaturedViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("backdropUrl")]
        public string? BackdropUrl { get; set; }
        [JsonPropertyName("trailerUrl")]
        public string? TrailerUrl { get; set; }
    }
}
