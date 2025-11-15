namespace BookingTicketCinema.WebApp.ViewModel
{
    public class PaymentRequestDto
    {
        public int ShowtimeId { get; set; }
        public List<int> SeatIds { get; set; } = new();
    }
}
