using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingTicketCinema.Models
{
    public class Seat : BaseEntity
    {
        [Key]
        public int SeatId { get; set; }

        [Required, MaxLength(10)]
        public string SeatNumber { get; set; } = null!; // A1, B2, ...

        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        [ForeignKey("SeatGroup")]
        public int SeatGroupId { get; set; }
        public SeatGroup SeatGroup { get; set; } = null!;

        // Navigation
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
