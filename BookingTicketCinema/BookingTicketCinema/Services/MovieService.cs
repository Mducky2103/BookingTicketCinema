using BookingTicketCinema.DTO;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using BookingTicketCinema.Services.Interface;

namespace BookingTicketCinema.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<IEnumerable<MovieCardDto>> GetNowShowingMoviesAsync()
        {
            var movies = await _movieRepository.GetNowShowingMoviesAsync(DateTime.Today);

            // Map Model sang DTO (Dùng Genre dạng string)
            return movies.Select(m => new MovieCardDto
            {
                Id = m.MovieId,
                Title = m.Title,
                PosterUrl = m.PosterUrl,
                BackdropUrl = m.BackdropUrl,
                Duration = (int)m.Duration.TotalMinutes,
                Genre = m.Genre 
            });
        }

        public async Task<IEnumerable<MovieCardDto>> GetComingSoonMoviesAsync()
        {
            var movies = await _movieRepository.GetComingSoonMoviesAsync(DateTime.Today);
            return movies.Select(m => new MovieCardDto
            {
                Id = m.MovieId,
                Title = m.Title,
                PosterUrl = m.PosterUrl,
                BackdropUrl = m.BackdropUrl,
                Duration = (int)m.Duration.TotalMinutes,
                Genre = m.Genre
            });
        }
        public async Task<IEnumerable<MovieFeaturedDto>> GetFeaturedMoviesAsync()
        {
            var movies = await _movieRepository.GetFeaturedMoviesAsync();
            return movies.Select(m => new MovieFeaturedDto
            {
                Id = m.MovieId,
                Title = m.Title,
                PosterUrl = m.PosterUrl,
                BackdropUrl = m.BackdropUrl,
                TrailerUrl = m.TrailerUrl
            });
        }
    }
}
