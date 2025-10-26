using BookingTicketCinema.Models;

namespace BookingTicketCinema.DTO
{
    public class CreateSeatGroupDto
    {
        public string GroupName { get; set; } = null!;
        public SeatGroup.SeatType Type { get; set; }
        public int RoomId { get; set; }
    }

    public class UpdateSeatGroupDto
    {
        public string? GroupName { get; set; }
        public SeatGroup.SeatType? Type { get; set; }
    }

    public class SeatGroupResponseDto
    {
        public int SeatGroupId { get; set; }
        public string GroupName { get; set; } = null!;
        public SeatGroup.SeatType Type { get; set; }
        public int RoomId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}


