namespace BookingTicketCinema.DTO
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? PosterUrl { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Duration { get; set; }
        public string? Genre { get; set; }
    }
}
