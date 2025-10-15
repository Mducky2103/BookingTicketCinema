using System.ComponentModel.DataAnnotations;

namespace BookingTicketCinema.Models
{
    public class Promotion : BaseEntity
    {
        [Key]
        public int PromotionId { get; set; }

        [Required, MaxLength(100)]
        public string Code { get; set; } = null!; // Mã giảm giá

        [Required]
        public decimal DiscountPercent { get; set; } // VD: 0.1 = 10%

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<VoucherRedemption> VoucherRedemptions { get; set; } = new List<VoucherRedemption>();
    }
}
