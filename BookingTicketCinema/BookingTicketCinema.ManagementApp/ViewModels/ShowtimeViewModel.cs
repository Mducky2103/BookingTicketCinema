namespace BookingTicketCinema.ManagementApp.ViewModels
{
    public class ShowtimeViewModel
    {
        public int ShowtimeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? MovieName { get; set; }
        public string? RoomName { get; set; }
        public int TicketsSold { get; set; }
        public int TotalSeats { get; set; }
    }
}
