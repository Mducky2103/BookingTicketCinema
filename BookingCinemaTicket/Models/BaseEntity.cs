using System.ComponentModel.DataAnnotations;

namespace BookingCinemaTicket.Models
{
    public class BaseEntity
    {
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
