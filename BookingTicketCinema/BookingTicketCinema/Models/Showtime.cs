using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingTicketCinema.Models
{
    public class Showtime : BaseEntity
    {
        [Key]
        public int ShowtimeId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [ForeignKey("Movie")]
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<PriceRule> PriceRules { get; set; } = new List<PriceRule>();
    }
}
