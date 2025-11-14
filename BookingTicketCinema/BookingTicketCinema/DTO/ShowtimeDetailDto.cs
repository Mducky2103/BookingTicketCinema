namespace BookingTicketCinema.DTO
{
    public class ShowtimeDetailDto
    {
        public int ShowtimeId { get; set; }
        public DateTime StartTime { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int RoomType { get; set; }
    }
}
