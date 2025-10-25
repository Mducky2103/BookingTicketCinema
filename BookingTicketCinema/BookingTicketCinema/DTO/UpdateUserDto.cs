using Org.BouncyCastle.Utilities;

namespace BookingTicketCinema.DTO
{
    public class UpdateUserDto
    {
        public string Email { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? PhoneNumber { get; set; }
    }
}
