using System.ComponentModel.DataAnnotations;

namespace BookingTicketCinema.DTO
{
    public class UserRegistrationModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).{6,}$",
           ErrorMessage = "Password must contain at least one uppercase letter and one special character.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; }
        [Required(ErrorMessage = "Date of Birth is required.")]
        public string Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
    }
}
