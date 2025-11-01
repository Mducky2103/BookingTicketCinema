using Microsoft.AspNetCore.Mvc;

namespace BookingTicketCinema.ManagementApp.Controllers;

public class AdminController : Controller
{
  [Route("admin/dashboard")]
  public IActionResult Dashboard() => View();
}

