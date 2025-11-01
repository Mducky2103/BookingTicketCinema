using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinemaClient.Controllers;

public class AdminController : Controller
{
  [Route("admin/dashboard")]
  public IActionResult Dashboard() => View();
}

