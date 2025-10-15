using System.ComponentModel.DataAnnotations;

namespace BookingTicketCinema.Models
{
    public abstract class BaseEntity
    {
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
