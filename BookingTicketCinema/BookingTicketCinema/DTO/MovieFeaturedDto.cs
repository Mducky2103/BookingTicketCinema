namespace BookingTicketCinema.DTO
{
    public class MovieFeaturedDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? PosterUrl { get; set; }
        public string? BackdropUrl { get; set; }
        public string? TrailerUrl { get; set; }
    }
}
