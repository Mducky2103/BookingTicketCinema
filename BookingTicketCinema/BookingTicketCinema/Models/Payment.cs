using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingTicketCinema.Models
{
    public class Payment : BaseEntity
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod Method { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }

        [ForeignKey("Ticket")]
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;

        public enum PaymentMethod
        {
            Cash = 0,
            CreditCard = 1,
            Online = 2
        }

        public enum PaymentStatus
        {
            Pending = 0,
            Completed = 1,
            Failed = 2
        }
    }
}
