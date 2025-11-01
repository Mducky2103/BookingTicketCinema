using System.Text.Json.Serialization;

namespace BookingTicketCinema.ManagementApp.ViewModels
{
    public class UserProfileModel
    {
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }
        [JsonPropertyName("gender")]
        public string? Gender { get; set; }
        [JsonPropertyName("dateOfBirth")]
        public DateOnly? DOB { get; set; }
    }
}
