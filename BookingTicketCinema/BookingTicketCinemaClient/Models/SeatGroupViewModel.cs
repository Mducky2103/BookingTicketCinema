namespace BookingTicketCinemaClient.Models;

public class SeatGroupViewModel
{
    public int SeatGroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public int Type { get; set; }
    public string TypeName => Type switch
    {
        0 => "Standard",
        1 => "VIP",
        2 => "Couple",
        _ => "Unknown"
    };
    public int RoomId { get; set; }
    public string? RoomName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSeatGroupRequest
{
    public string GroupName { get; set; } = null!;
    public int Type { get; set; }
    public int RoomId { get; set; }
}

public class UpdateSeatGroupRequest
{
    public string? GroupName { get; set; }
    public int? Type { get; set; }
}

