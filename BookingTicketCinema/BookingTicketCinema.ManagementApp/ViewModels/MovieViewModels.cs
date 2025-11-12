using System.Text.Json.Serialization;

namespace BookingTicketCinema.ManagementApp.ViewModels
{
    public class MovieViewModel
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
        [JsonPropertyName("status")]
        public int Status { get; set; } // 0, 1, 2

        public string StatusText => Status switch
        {
            0 => "Sắp chiếu",
            1 => "Đang chiếu",
            2 => "Đã kết thúc",
            _ => "Không rõ"
        };
    }
}
