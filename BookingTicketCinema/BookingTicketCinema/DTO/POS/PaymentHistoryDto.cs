using BookingTicketCinema.DTO.Booking;
using static BookingTicketCinema.Models.Payment;

namespace BookingTicketCinema.DTO.POS
{
    public class PaymentHistoryDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public string MovieTitle { get; set; } = string.Empty;
        public string? PosterUrl { get; set; }
        public DateTime Showtime { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public List<TicketHistoryDto> TicketsInPayment { get; set; } = new();
    }
}
