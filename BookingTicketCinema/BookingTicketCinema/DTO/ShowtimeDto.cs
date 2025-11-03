using BookingTicketCinema.Models;
using System.ComponentModel.DataAnnotations;

namespace BookingTicketCinema.DTO
{
    public class ShowtimeResponseDto
    {
        public int ShowtimeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MovieId { get; set; }
        public string? MovieName { get; set; }
        public int RoomId { get; set; }
        public string? RoomName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}


