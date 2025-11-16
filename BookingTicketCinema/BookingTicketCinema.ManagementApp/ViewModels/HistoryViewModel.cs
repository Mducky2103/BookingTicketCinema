using System.Text.Json.Serialization;

namespace BookingTicketCinema.ManagementApp.ViewModels
{
    public class PagedResultViewModel<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; } = new();

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
    }

    public class PaymentHistoryViewModel
    {
        [JsonPropertyName("paymentId")]
        public int PaymentId { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; } // 0=Pending, 1=Completed, 2=Failed, 3=Cancelled

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("movieTitle")]
        public string MovieTitle { get; set; } = string.Empty;

        [JsonPropertyName("posterUrl")]
        public string? PosterUrl { get; set; }

        [JsonPropertyName("showtime")]
        public DateTime Showtime { get; set; }

        [JsonPropertyName("roomName")]
        public string RoomName { get; set; } = string.Empty;

        [JsonPropertyName("ticketsInPayment")]
        public List<TicketHistoryDto> TicketsInPayment { get; set; } = new();

        public string StatusText => Status switch
        {
            1 => "Đã thanh toán",
            2 => "Thất bại",
            3 => "Đã hủy",
            _ => "Đang chờ"
        };

        public string StatusClass => Status switch
        {
            1 => "success",
            2 => "danger",
            3 => "secondary",
            _ => "warning"
        };
    }

    public class TicketHistoryDto
    {
        [JsonPropertyName("ticketId")]
        public int TicketId { get; set; }

        [JsonPropertyName("paymentId")]
        public int PaymentId { get; set; }

        [JsonPropertyName("seat")]
        public string Seat { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}
