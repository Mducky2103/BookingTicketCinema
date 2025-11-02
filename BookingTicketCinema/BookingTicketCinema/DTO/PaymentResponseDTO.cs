using BookingTicketCinema.Controllers;

namespace BookingTicketCinema.DTO
{
    public class PaymentResponseDTO
    {
        public int TicketId { get; set; }
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatusEnum Status { get; set; }
    }
}
