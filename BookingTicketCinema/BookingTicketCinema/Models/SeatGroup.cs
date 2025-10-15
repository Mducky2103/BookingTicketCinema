using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingTicketCinema.Models
{
    public class SeatGroup : BaseEntity
    {
        [Key]
        public int SeatGroupId { get; set; }

        [Required, MaxLength(100)]
        public string GroupName { get; set; } = null!; // Ví dụ: Standard, VIP, Couple

        [Required]
        public SeatType Type { get; set; }

        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
        public ICollection<PriceRule> PriceRules { get; set; } = new List<PriceRule>();

        public enum SeatType
        {
            Standard = 0,
            VIP = 1,
            Couple = 2
        }
    }
}
