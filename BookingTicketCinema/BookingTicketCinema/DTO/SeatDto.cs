using BookingTicketCinema.Models;

namespace BookingTicketCinema.DTO
{
    public class CreateSeatDto
    {
        public string SeatNumber { get; set; } = null!;
        public Seat.SeatStatus Status { get; set; } = Seat.SeatStatus.Empty;
        public int RoomId { get; set; }
        public int SeatGroupId { get; set; }
    }

    public class UpdateSeatDto
    {
        public string? SeatNumber { get; set; }
        public Seat.SeatStatus? Status { get; set; }
        public int? SeatGroupId { get; set; }
    }

    public class SeatResponseDto
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public Seat.SeatStatus Status { get; set; }
        public int RoomId { get; set; }
        public int SeatGroupId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

