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

        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        // 1 Payment (Đơn hàng) sẽ có NHIỀU Vé
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

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
