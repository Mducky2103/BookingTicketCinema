using BookingTicketCinema.Models;

namespace BookingTicketCinema.DTO
{
    public class CreateRoomDto
    {
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
        public Room.RoomType Type { get; set; }
    }

    public class UpdateRoomDto
    {
        public string? Name { get; set; }
        public int? Capacity { get; set; }
        public Room.RoomType? Type { get; set; }
    }

    public class RoomResponseDto
    {
        public int RoomId { get; set; }
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
        public Room.RoomType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}


