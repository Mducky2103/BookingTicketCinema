using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BookingTicketCinema.Models
{
    public class User : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(10)")]
        public string Gender { get; set; }

        [PersonalData]
        public DateOnly DOB { get; set; }
    }
}
