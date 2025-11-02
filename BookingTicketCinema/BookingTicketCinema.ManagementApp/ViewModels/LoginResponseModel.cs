using System.Text.Json.Serialization;

namespace BookingTicketCinema.ManagementApp.ViewModels
{
    public class LoginResponseModel
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }
}
