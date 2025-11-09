namespace BookingTicketCinema.DTO
{
    public class MovieFeaturedDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        // Dùng PosterUrl làm ảnh nền (vì DB của bạn không có BackdropUrl)
        public string? BackdropUrl { get; set; }
        public string? TrailerUrl { get; set; }
    }
}
