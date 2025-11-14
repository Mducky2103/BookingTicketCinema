using System.Text.Json.Serialization;

namespace BookingTicketCinema.WebApp.ViewModel
{
    public class ShowtimeDetailViewModel
    {
        [JsonPropertyName("showtimeId")]
        public int ShowtimeId { get; set; }
        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; }
        [JsonPropertyName("roomName")]
        public string RoomName { get; set; } = string.Empty;
        [JsonPropertyName("roomType")]
        public int RoomType { get; set; }

        // Hàm helper để hiển thị loại phòng
        public string RoomTypeText => RoomType switch
        {
            1 => "IMAX",
            2 => "VIP",
            _ => "2D Phụ đề" 
        };
    }
}
