using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingTicketCinema.Models
{
    public class VoucherRedemption : BaseEntity
    {
        [Key]
        public int RedemptionId { get; set; }

        [Required]
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("Promotion")]
        public int PromotionId { get; set; }
        public Promotion Promotion { get; set; } = null!;

        [ForeignKey("User")]
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
