using BookingTicketCinema.Models;
using BookingTicketCinema.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace BookingTicketCinema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MovieForClientController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieForClientController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        // Endpoint mới cho Phim Nổi Bật
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeatured()
        {
            var movies = await _movieService.GetFeaturedMoviesAsync();
            return Ok(movies);
        }

        // Endpoint mới cho Phim Đang Chiếu
        [HttpGet("now-showing")]
        public async Task<IActionResult> GetNowShowing()
        {
            var movies = await _movieService.GetNowShowingMoviesAsync();
            return Ok(movies);
        }

        // Endpoint mới cho Phim Sắp Chiếu
        [HttpGet("coming-soon")]
        public async Task<IActionResult> GetComingSoon()
        {
            var movies = await _movieService.GetComingSoonMoviesAsync();
            return Ok(movies);
        }
    }
}
