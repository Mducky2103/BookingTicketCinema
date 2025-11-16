namespace BookingTicketCinema.DTO.Promotion
{
    public class PromotionDto
    {
        public int PromotionId { get; set; }
        public string Code { get; set; } = null!;
        public decimal DiscountPercent { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public int UsageCount { get; set; } 
    }
}
