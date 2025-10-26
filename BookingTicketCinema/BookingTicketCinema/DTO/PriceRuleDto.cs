using BookingTicketCinema.Models;

namespace BookingTicketCinema.DTO
{
    public class CreatePriceRuleDto
    {
        public decimal BasePrice { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public PriceRule.TimeSlot Slot { get; set; }
        public int SeatGroupId { get; set; }
        public int? ShowtimeId { get; set; }
    }

    public class UpdatePriceRuleDto
    {
        public decimal? BasePrice { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public PriceRule.TimeSlot? Slot { get; set; }
        public int? SeatGroupId { get; set; }
        public int? ShowtimeId { get; set; }
    }

    public class PriceRuleResponseDto
    {
        public int PriceRuleId { get; set; }
        public decimal BasePrice { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public PriceRule.TimeSlot Slot { get; set; }
        public int SeatGroupId { get; set; }
        public int? ShowtimeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

