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
        
        public async Task<IActionResult> CreateMovie([FromForm] MovieCreateRequest request)
        {
            // ✅ Validate Title
            if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Trim().ToLower() == "string")
                return BadRequest("Title is required and cannot be 'string'.");

            var movie = new Movie
            {
                Title = request.Title,
                Description = request.Description,
                Genre = request.Genre,
                TrailerUrl = request.TrailerUrl,
            };

            // ✅ Duration
            if (string.IsNullOrWhiteSpace(request.Duration) || request.Duration.Trim().ToLower() == "string")
                return BadRequest("Duration is required.");
            if (!TimeSpan.TryParse(request.Duration, out var ts))
                return BadRequest("Invalid Duration format. Please use H:mm:ss (e.g. 1:30:00).");
            movie.Duration = ts;

            // ✅ ReleaseDate
            if (string.IsNullOrWhiteSpace(request.ReleaseDate) || request.ReleaseDate.Trim().ToLower() == "string")
                return BadRequest("ReleaseDate is required.");
            if (!DateOnly.TryParse(request.ReleaseDate, out var d))
                return BadRequest("Invalid ReleaseDate format. Please use yyyy-MM-dd (e.g. 2025-10-24).");
            movie.ReleaseDate = d;

            // ✅ Validate Status
            if (!request.Status.HasValue)
                return BadRequest("Status is required.");
            if (request.Status.Value < 0 || request.Status.Value > 2)
                return BadRequest("Invalid Status. Allowed values: 0=ComingSoon, 1=NowShowing, 2=Ended.");
            movie.Status = (MovieStatus)request.Status.Value;

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

            if(request.BackdropFile != null && request.BackdropFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "backdrops");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                var fileExt = Path.GetExtension(request.BackdropFile.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExt}";
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.BackdropFile.CopyToAsync(stream);
                }
                movie.BackdropUrl = $"/backdrops/{fileName}";
            }

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovie), new { id = movie.MovieId }, movie);
        }

        [HttpPut("{id}")]
        
        public async Task<IActionResult> UpdateMovie(int id, [FromForm] MovieUpdateRequest request)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound("Movie not found.");

            // ✅ Title
            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                if (request.Title.Trim().ToLower() == "string")
                    return BadRequest("Invalid title value.");
                movie.Title = request.Title;
            }

            // ✅ Description, Genre, Trailer
            if (!string.IsNullOrWhiteSpace(request.Description) && request.Description.Trim().ToLower() != "string")
                movie.Description = request.Description;

            if (!string.IsNullOrWhiteSpace(request.Genre) && request.Genre.Trim().ToLower() != "string")
                movie.Genre = request.Genre;

            if (!string.IsNullOrWhiteSpace(request.TrailerUrl) && request.TrailerUrl.Trim().ToLower() != "string")
                movie.TrailerUrl = request.TrailerUrl;

            // ✅ Duration
            if (!string.IsNullOrWhiteSpace(request.Duration))
            {
                if (request.Duration.Trim().ToLower() == "string")
                    return BadRequest("Invalid Duration value.");
                if (!TimeSpan.TryParse(request.Duration, out var ts))
                    return BadRequest("Invalid Duration format. Please use H:mm:ss (e.g. 2:15:00).");
                movie.Duration = ts;
            }

            // ✅ ReleaseDate
            if (!string.IsNullOrWhiteSpace(request.ReleaseDate))
            {
                if (request.ReleaseDate.Trim().ToLower() == "string")
                    return BadRequest("Invalid ReleaseDate value.");
                if (!DateOnly.TryParse(request.ReleaseDate, out var d))
                    return BadRequest("Invalid ReleaseDate format. Please use yyyy-MM-dd (e.g. 2025-10-24).");
                movie.ReleaseDate = d;
            }

            // ✅ Status
            if (request.Status.HasValue)
            {
                if (request.Status.Value < 0 || request.Status.Value > 2)
                    return BadRequest("Invalid Status. Allowed values: 0=ComingSoon, 1=NowShowing, 2=Ended.");
                movie.Status = (Movie.MovieStatus)request.Status.Value;
            }

            // ✅ Poster upload
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
            // ✅ Backdrop upload
            if (request.BackdropFile != null && request.BackdropFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "backdrops");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                var fileExt = Path.GetExtension(request.BackdropFile.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExt}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.BackdropFile.CopyToAsync(stream);
                }

                movie.BackdropUrl = $"/backdrops/{fileName}";
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
