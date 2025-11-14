namespace BookingTicketCinema.DTO
{
    public class MovieCreateRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Genre { get; set; }
        public string? Duration { get; set; }
        public string? ReleaseDate { get; set; }
        public int? Status { get; set; }
        public string? TrailerUrl { get; set; }
        public IFormFile? PosterFile { get; set; }
        public IFormFile? BackdropFile { get; set; }
    }

}
