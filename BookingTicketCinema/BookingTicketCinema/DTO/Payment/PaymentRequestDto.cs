namespace BookingTicketCinema.DTO.Payment
{
    public class PaymentRequestDto
    {
        public int ShowtimeId { get; set; }
        public List<int> SeatIds { get; set; } = new();
    }
}
