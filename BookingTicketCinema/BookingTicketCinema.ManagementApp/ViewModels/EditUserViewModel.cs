using System.ComponentModel.DataAnnotations;

namespace BookingTicketCinema.ManagementApp.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
