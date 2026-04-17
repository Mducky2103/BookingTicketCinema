using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingTicketCinema.Models
{
    public class Seat : BaseEntity
    {
        [Key]
        public int SeatId { get; set; }

        [Required, MaxLength(10)]
        public string SeatNumber { get; set; } = null!;

        // Tọa độ để vẽ lên Grid
        [Required]
        public int RowIndex { get; set; }   
        [Required]
        public int ColumnIndex { get; set; } 

        [Required]
        public SeatStatus Status { get; set; } = SeatStatus.Empty;

        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        [ForeignKey("SeatGroup")]
        public int SeatGroupId { get; set; }
        public SeatGroup SeatGroup { get; set; } = null!;

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        public enum SeatStatus
        {
            Empty = 0,
            Booked = 1,
            Broken = 2
        }
    }
}
