namespace BookingTicketCinema.ManagementApp.ViewModels
{
    public class POSRequestViewModel
    {
        public int ShowtimeId { get; set; }
        public List<int> SeatIds { get; set; } = new();
        public int Method { get; set; }
    }
}
