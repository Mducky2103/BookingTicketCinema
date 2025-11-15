using static BookingTicketCinema.Models.Ticket;

namespace BookingTicketCinema.DTO.Booking
{
    public class TicketHistoryDto
    {
        public int TicketId { get; set; }
        public string Movie { get; set; } = string.Empty;
        public string? PosterUrl { get; set; } 
        public DateTime StartTime { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string Seat { get; set; } = string.Empty;
        public TicketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
