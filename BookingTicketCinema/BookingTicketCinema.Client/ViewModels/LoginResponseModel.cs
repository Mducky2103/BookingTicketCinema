using System.Text.Json.Serialization;

namespace BookingTicketCinema.Client.ViewModels
{
    public class LoginResponseModel
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }
}
