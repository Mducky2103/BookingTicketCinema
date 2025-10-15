using System.ComponentModel.DataAnnotations;

namespace BookingCinemaTicket.Models
{
    public class Movie : BaseEntity
    {
        [Key]
        public int MovieId { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = null!;

        [MaxLength(255)]
        public string? PosterUrl { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        public MovieStatus Status { get; set; }

        [MaxLength(100)]
        public string? Genre { get; set; }

        public DateOnly ReleaseDate { get; set; }

        // Relationships
        //public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();

        public enum MovieStatus
        {
            ComingSoon = 0,
            NowShowing = 1,
            Ended = 2
        }
    }
}
