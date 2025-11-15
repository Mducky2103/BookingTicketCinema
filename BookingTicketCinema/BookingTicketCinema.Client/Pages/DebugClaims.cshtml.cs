using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookingTicketCinema.WebApp.Pages
{
    public class DebugClaimsModel : PageModel
    {
        public List<Tuple<string, string>> Claims { get; set; } = new();

        public void OnGet()
        {
            // Lấy TẤT CẢ claims từ Cookie của user
            foreach (var claim in User.Claims)
            {
                Claims.Add(new Tuple<string, string>(claim.Type, claim.Value));
            }
        }
    }
}
