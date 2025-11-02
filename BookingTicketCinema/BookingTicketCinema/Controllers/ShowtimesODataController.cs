using System.Linq;
using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;

namespace BookingTicketCinema.Controllers
{
    // Exposed at /odata/Showtimes
    public class ShowtimesController : ODataController
    {
        private readonly CinemaDbContext _context;

        public ShowtimesController(CinemaDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IQueryable<Showtime> Get()
        {
            // returning IQueryable allows OData to apply filter/orderby/skip/top/expand
            return _context.Showtimes.AsQueryable();
        }
    }
}
