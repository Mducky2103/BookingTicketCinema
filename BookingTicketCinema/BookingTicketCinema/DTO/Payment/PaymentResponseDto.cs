using static BookingTicketCinema.Models.Payment;

namespace BookingTicketCinema.DTO.Payment
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public DateTime Showtime { get; set; }
        public List<string> SeatNumbers { get; set; } = new();
    }
}
