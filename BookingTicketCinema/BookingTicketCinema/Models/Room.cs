using System.ComponentModel.DataAnnotations;

namespace BookingTicketCinema.Models
{
    public class Room : BaseEntity
    {
        [Key]
        public int RoomId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        public int Capacity { get; set; }

        // Navigation
        public ICollection<SeatGroup> SeatGroups { get; set; } = new List<SeatGroup>();
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
        public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
    }
}
