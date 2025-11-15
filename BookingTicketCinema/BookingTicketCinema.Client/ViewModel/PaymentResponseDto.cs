using System.Text.Json.Serialization;

namespace BookingTicketCinema.WebApp.ViewModel
{
    public class PaymentResponseDto
    {
        [JsonPropertyName("paymentId")]
        public int PaymentId { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("movieTitle")]
        public string MovieTitle { get; set; } = string.Empty;

        [JsonPropertyName("roomName")]
        public string RoomName { get; set; } = string.Empty;

        [JsonPropertyName("showtime")]
        public DateTime Showtime { get; set; }

        [JsonPropertyName("seatNumbers")]
        public List<string> SeatNumbers { get; set; } = new();
        public string StatusText => Status switch
        {
            1 => "Đã thanh toán",
            2 => "Thất bại",
            _ => "Đang chờ"
        };
    }
}
