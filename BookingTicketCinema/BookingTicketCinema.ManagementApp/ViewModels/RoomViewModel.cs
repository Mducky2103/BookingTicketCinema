namespace BookingTicketCinema.ManagementApp.Models;

public class RoomViewModel
{
    public int RoomId { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public int Type { get; set; }
    public string TypeName => Type switch
    {
        0 => "2D",
        1 => "3D",
        2 => "IMAX",
        _ => "Unknown"
    };
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateRoomRequest
{
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public int Type { get; set; }
}

public class UpdateRoomRequest
{
    public string? Name { get; set; }
    public int? Capacity { get; set; }
    public int? Type { get; set; }
}

