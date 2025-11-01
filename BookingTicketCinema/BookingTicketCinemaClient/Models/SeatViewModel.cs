namespace BookingTicketCinemaClient.Models;

public class SeatViewModel
{
    public int SeatId { get; set; }
    public string SeatNumber { get; set; } = null!;
    public int Status { get; set; }
    public string StatusName => Status switch
    {
        0 => "Empty",
        1 => "Booked",
        2 => "Broken",
        _ => "Unknown"
    };
    public int RoomId { get; set; }
    public string? RoomName { get; set; }
    public int SeatGroupId { get; set; }
    public string? SeatGroupName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSeatRequest
{
    public string SeatNumber { get; set; } = null!;
    public int Status { get; set; } = 0;
    public int RoomId { get; set; }
    public int SeatGroupId { get; set; }
}

public class UpdateSeatRequest
{
    public string? SeatNumber { get; set; }
    public int? Status { get; set; }
    public int? SeatGroupId { get; set; }
}

