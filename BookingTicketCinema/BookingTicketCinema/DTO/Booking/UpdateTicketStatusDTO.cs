using static BookingTicketCinema.Models.Ticket;

namespace BookingTicketCinema.DTO.Booking
{
    public class UpdateTicketStatusDTO
    {
        public TicketStatus Status { get; set; }
    }
}
