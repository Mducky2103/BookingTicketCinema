using BookingTicketCinema.Data;
using BookingTicketCinema.DTO;
using BookingTicketCinema.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BookingTicketCinema.Models.Movie;

namespace BookingTicketCinema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly CinemaDbContext _context;

        public MovieController(CinemaDbContext context)
        {
            _context = context;
        }

        // GET: api/movie
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            return await _context.Movies.ToListAsync();
        }

        // GET: api/movie/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [AllowAnonymous] // chỉ để test, sau này remove
        public async Task<IActionResult> CreateMovie([FromForm] MovieCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return BadRequest("Title is required.");

            var movie = new Movie
            {
                Title = request.Title,
                Description = request.Description,
                Genre = request.Genre,
                TrailerUrl = request.TrailerUrl,
            };

            // ✅ Xử lý Duration an toàn
            if (string.IsNullOrWhiteSpace(request.Duration))
            {
                if (TimeSpan.TryParse(request.Duration, out var ts))
                    movie.Duration = ts;
                else
                    return BadRequest("Invalid Duration format. Please use H:mm:ss (e.g. 1:30:00).");
            }

            // ✅ Xử lý ReleaseDate an toàn
            if (string.IsNullOrWhiteSpace(request.ReleaseDate))
            {
                if (DateOnly.TryParse(request.ReleaseDate, out var d))
                    movie.ReleaseDate = d;
                else
                    return BadRequest("Invalid ReleaseDate format. Please use yyyy-MM-dd (e.g. 2025-10-24).");
            }

            // ✅ Validate Status (chỉ cho phép 0, 1, 2)
            if (request.Status.HasValue)
            {
                if (request.Status.Value < 0 || request.Status.Value > 2)
                    return BadRequest("Invalid Status. Allowed values: 0=ComingSoon, 1=NowShowing, 2=Ended.");

                movie.Status = (MovieStatus)request.Status.Value;
            }

            // ✅ Upload poster (nếu có)
            if (request.PosterFile != null && request.PosterFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "posters");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var fileExt = Path.GetExtension(request.PosterFile.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExt}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.PosterFile.CopyToAsync(stream);
                }

                movie.PosterUrl = $"/posters/{fileName}";
            }

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovie), new { id = movie.MovieId }, movie);
        }

        [HttpPut("{id}")]
        [AllowAnonymous] // chỉ để test, sau này bỏ đi
        public async Task<IActionResult> UpdateMovie(int id, [FromForm] MovieUpdateRequest request)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound("Movie not found.");
            }

            if (!string.IsNullOrWhiteSpace(request.Title))
                movie.Title = request.Title;

            if (!string.IsNullOrWhiteSpace(request.Description))
                movie.Description = request.Description;

            if (!string.IsNullOrWhiteSpace(request.Genre))
                movie.Genre = request.Genre;

            if (!string.IsNullOrWhiteSpace(request.TrailerUrl))
                movie.TrailerUrl = request.TrailerUrl;

            // ✅ Xử lý Duration an toàn
            if (string.IsNullOrWhiteSpace(request.Duration))
            {
                if (TimeSpan.TryParse(request.Duration, out var ts))
                    movie.Duration = ts;
                else
                    return BadRequest("Invalid Duration format. Please use H:mm:ss (e.g. 2:15:00).");
            }

            // ✅ Xử lý ReleaseDate an toàn
            if (string.IsNullOrWhiteSpace(request.ReleaseDate))
            {
                if (DateOnly.TryParse(request.ReleaseDate, out var d))
                    movie.ReleaseDate = d;
                else
                    return BadRequest("Invalid ReleaseDate format. Please use yyyy-MM-dd (e.g. 2025-10-24).");
            }

            // ✅ Validate Status (chỉ cho phép 0, 1, 2)
            if (request.Status.HasValue)
            {
                if (request.Status.Value < 0 || request.Status.Value > 2)
                    return BadRequest("Invalid Status. Allowed values: 0=ComingSoon, 1=NowShowing, 2=Ended.");

                movie.Status = (Movie.MovieStatus)request.Status.Value;
            }

            // ✅ Upload file mới (nếu có)
            if (request.PosterFile != null && request.PosterFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "posters");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var fileExt = Path.GetExtension(request.PosterFile.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExt}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.PosterFile.CopyToAsync(stream);
                }

                movie.PosterUrl = $"/posters/{fileName}";
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Movie updated successfully.",
                movie
            });
        }


        // DELETE: api/movie/5
        [HttpDelete("{id}")]
        [AllowAnonymous] // cho phép test
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return Ok("Movie has been removed");
        }
    }
}
