using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.Client.Pages.Login
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        public LoginModel(ILogger<LoginModel> logger)
        {
            _logger = logger;
        }
        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;
        public void OnGet()
        {
        }
    }
}
