using System.ComponentModel.DataAnnotations;

namespace BookingTicketCinema.DTO.Promotion
{
    public class PromotionCreateDto
    {
        [Required, MaxLength(100)]
        public string Code { get; set; } = null!;

        [Required]
        [Range(0.01, 1.0)] // Giảm giá phải từ 1% đến 100%
        public decimal DiscountPercent { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
