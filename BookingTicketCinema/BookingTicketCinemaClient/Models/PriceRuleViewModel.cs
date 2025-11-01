namespace BookingTicketCinemaClient.Models;

public class PriceRuleViewModel
{
    public int PriceRuleId { get; set; }
    public decimal BasePrice { get; set; }
    public int DayOfWeek { get; set; }
    public string DayOfWeekName => ((System.DayOfWeek)DayOfWeek).ToString();
    public int Slot { get; set; }
    public string SlotName => Slot switch
    {
        0 => "Morning",
        1 => "Afternoon",
        2 => "Evening",
        3 => "Late Night",
        _ => "Unknown"
    };
    public int SeatGroupId { get; set; }
    public string? SeatGroupName { get; set; }
    public int? ShowtimeId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreatePriceRuleRequest
{
    public decimal BasePrice { get; set; }
    public int DayOfWeek { get; set; }
    public int Slot { get; set; }
    public int SeatGroupId { get; set; }
    public int? ShowtimeId { get; set; }
}

public class UpdatePriceRuleRequest
{
    public decimal? BasePrice { get; set; }
    public int? DayOfWeek { get; set; }
    public int? Slot { get; set; }
    public int? SeatGroupId { get; set; }
    public int? ShowtimeId { get; set; }
}

