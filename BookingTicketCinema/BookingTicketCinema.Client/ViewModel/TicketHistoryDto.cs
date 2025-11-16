using System.Text.Json.Serialization;

namespace BookingTicketCinema.WebApp.ViewModel
{
    public class TicketHistoryDto
    {
        [JsonPropertyName("ticketId")]
        public int TicketId { get; set; }

        [JsonPropertyName("paymentId")]
        public int PaymentId { get; set; }

        [JsonPropertyName("movie")]
        public string Movie { get; set; } = string.Empty;

        [JsonPropertyName("posterUrl")]
        public string? PosterUrl { get; set; }

        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("roomName")]
        public string RoomName { get; set; } = string.Empty;

        [JsonPropertyName("seat")]
        public string Seat { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public int Status { get; set; } // 0=Pending, 1=Booked, 2=Cancelled

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        // Hàm helper để hiển thị chữ (dựa trên int)
        public string StatusText => Status switch
        {
            2 => "Đã thanh toán",       
            1 => "Đang chờ thanh toán",
            3 => "Đã hủy",              
            0 => "Còn trống",           
            _ => "Không xác định"
        };

        // Hàm helper để tạo màu badge
        public string StatusClass => Status switch
        {
            2 => "success", 
            1 => "warning",
            3 => "danger", 
            _ => "secondary"
        };
    }
}
