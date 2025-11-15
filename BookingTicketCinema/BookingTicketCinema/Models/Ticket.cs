using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingTicketCinema.Models
{
    public class Ticket : BaseEntity
    {
        [Key]
        public int TicketId { get; set; }

        [Required]
        public TicketStatus Status { get; set; } = TicketStatus.Available;

        [ForeignKey("Showtime")]
        public int ShowtimeId { get; set; }
        public Showtime Showtime { get; set; } = null!;

        [ForeignKey("Seat")]
        public int SeatId { get; set; }
        public Seat Seat { get; set; } = null!;

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public User? User { get; set; }

        [ForeignKey("Payment")]
        public int? PaymentId { get; set; } 
        public Payment? Payment { get; set; }

        public enum TicketStatus
        {
            Available = 0,
            Reserved = 1,
            Paid = 2,
            Cancelled = 3
        }
    }
}
