using System.Text.Json.Serialization;

namespace BookingTicketCinema.ManagementApp.ViewModels
{
    public class UserListViewModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("gender")]
        public string Gender { get; set; } = string.Empty;

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;
    }
}
