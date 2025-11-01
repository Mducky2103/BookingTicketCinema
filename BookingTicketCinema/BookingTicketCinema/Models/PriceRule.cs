using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingTicketCinema.Models
{
    public class PriceRule : BaseEntity
    {
        [Key]
        public int PriceRuleId { get; set; }
        
        [Required]
        public decimal BasePrice { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; } 
        
        [Required]
        public TimeSlot Slot { get; set; } 
        
        [ForeignKey("SeatGroup")]
        public int SeatGroupId { get; set; } 
        public SeatGroup SeatGroup { get; set; } = null!;
        public int? ShowtimeId { get; set; }
        public Showtime? Showtime { get; set; }

        public enum TimeSlot
        {
            Morning = 0,
            Afternoon = 1,
            Evening = 2,
            LateNight = 3
        }
    }
}
