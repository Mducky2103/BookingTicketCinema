namespace BookingTicketCinema.WebApp.ViewModel
{
    public class BookingRequestViewModel
    {
        public int ShowtimeId { get; set; }
        public List<int> SeatIds { get; set; } = new();
    }
}
