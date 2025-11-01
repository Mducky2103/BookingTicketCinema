namespace BookingTicketCinema.DTO
{
    public class UserRegistrationModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
    }
}
