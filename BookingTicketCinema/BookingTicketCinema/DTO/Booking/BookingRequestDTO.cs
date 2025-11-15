namespace BookingTicketCinema.DTO.Booking
{
    public class BookingRequestDTO
    {
        public int ShowtimeId { get; set; }
        public List<int> SeatIds { get; set; } = new();
    }
}
