using System.Text.Json.Serialization;

namespace BookingTicketCinema.WebApp.ViewModel
{
    public class SeatGroupViewModel
    {
        [JsonPropertyName("seatGroupId")]
        public int SeatGroupId { get; set; }
        [JsonPropertyName("groupName")]
        public string GroupName { get; set; } = string.Empty;
        [JsonPropertyName("type")]
        public int Type { get; set; } // 0=Thường, 1=VIP, 2=Đôi
    }
}
