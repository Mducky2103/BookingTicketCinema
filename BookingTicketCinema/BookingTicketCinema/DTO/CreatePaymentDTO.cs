using BookingTicketCinema.Models;

namespace BookingTicketCinema.DTO
{
    public class CreatePaymentDTO
    {
        public List<int> TicketIds { get; set; } = new();
        public decimal Amount { get; set; } = 0m;
        public Payment.PaymentMethod Method { get; set; } = Payment.PaymentMethod.Online;
    }
}
