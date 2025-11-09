namespace BookingTicketCinema.DTO
{
    public class TicketResponseDTO
    {
        public int TicketId { get; set; }
        public int SeatId { get; set; }
        public int ShowtimeId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
