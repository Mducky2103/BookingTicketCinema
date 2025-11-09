using System.ComponentModel.DataAnnotations;
using Org.BouncyCastle.Utilities;

namespace BookingTicketCinema.DTO
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Full name is required")]
        [MinLength(2, ErrorMessage = "Full name must be at least 2 characters long")]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "Invalid phone number")]
        [RegularExpression(@"^\d{10,11}$",
            ErrorMessage = "Phone number must contain 10 to 11 digits")]
        public string? PhoneNumber { get; set; }
    }
}
