using System.ComponentModel.DataAnnotations;

namespace BookingTicketCinema.DTO.Promotion
{
    public class PromotionUpdateDto
    {
        [Required]
        [Range(0.01, 1.0)]
        public decimal DiscountPercent { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
