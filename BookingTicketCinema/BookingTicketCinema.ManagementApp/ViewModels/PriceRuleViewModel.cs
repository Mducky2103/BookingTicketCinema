namespace BookingTicketCinema.ManagementApp.Models;

public class PriceRuleViewModel
{
    public int PriceRuleId { get; set; }
    public decimal BasePrice { get; set; }
    public int DayOfWeek { get; set; }
    public string DayOfWeekName => DayOfWeek switch
    {
        0 => "Chủ nhật",
        1 => "Thứ hai",
        2 => "Thứ ba",
        3 => "Thứ tư",
        4 => "Thứ năm",
        5 => "Thứ sáu",
        6 => "Thứ bảy",
        _ => "Unknown"
    };
    public int Slot { get; set; }
    public string SlotName => Slot switch
    {
        0 => "Sáng",
        1 => "Chiều",
        2 => "Tối",
        3 => "Khuya",
        _ => "Unknown"
    };
    public int SeatGroupId { get; set; }
    public string? SeatGroupName { get; set; }
    public int? ShowtimeId { get; set; }
    public string? ShowtimeName { get; set; }
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

