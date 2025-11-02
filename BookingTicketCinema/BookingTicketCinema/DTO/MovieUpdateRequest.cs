namespace BookingTicketCinema.DTO
{
    public class MovieUpdateRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Genre { get; set; }
        public string? Duration { get; set; }  // nhập dạng "02:15:00" hoặc "2:15"
        public string? ReleaseDate { get; set; } // yyyy-MM-dd
        public int? Status { get; set; } // enum int
        public string? TrailerUrl { get; set; }
        public IFormFile? PosterFile { get; set; } // upload ảnh mới (tùy chọn)
    }
}
