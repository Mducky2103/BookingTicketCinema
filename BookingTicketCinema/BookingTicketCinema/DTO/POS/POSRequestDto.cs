using static BookingTicketCinema.Models.Payment;

namespace BookingTicketCinema.DTO.POS
{
    public class POSRequestDto
    {
        public int ShowtimeId { get; set; }
        public List<int> SeatIds { get; set; } = new();
        public PaymentMethod Method { get; set; }
    }
}
