using System.Text.Json.Serialization;

namespace BookingTicketCinema.ManagementApp.ViewModels
{
    public class PromotionViewModel
    {
        [JsonPropertyName("promotionId")]
        public int PromotionId { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; } = null!;
        [JsonPropertyName("discountPercent")]
        public decimal DiscountPercent { get; set; }
        [JsonPropertyName("startDate")]
        public DateOnly StartDate { get; set; }
        [JsonPropertyName("endDate")]
        public DateOnly EndDate { get; set; }
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("usageCount")]
        public int UsageCount { get; set; }
    }

    public class PromotionEditViewModel
    {
        [JsonPropertyName("promotionId")]
        public int PromotionId { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; } = null!; 
        [JsonPropertyName("discountPercent")]
        public decimal DiscountPercent { get; set; }
        [JsonPropertyName("startDate")]
        public DateOnly StartDate { get; set; }
        [JsonPropertyName("endDate")]
        public DateOnly EndDate { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }
}
