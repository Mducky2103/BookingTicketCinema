using System.Linq;
using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;

namespace BookingTicketCinema.Controllers
{
    // Exposed at /odata/Movies
    public class MoviesController : ODataController
    {
        private readonly CinemaDbContext _context;

        public MoviesController(CinemaDbContext context)
        {
            _context = context;
        }

        [EnableQuery]
        public IQueryable<Movie> Get()
        {
            return _context.Movies.AsQueryable();
        }
    }
}
