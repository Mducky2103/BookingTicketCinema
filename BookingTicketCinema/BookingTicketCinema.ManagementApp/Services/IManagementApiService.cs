using BookingTicketCinema.ManagementApp.ViewModels;

namespace BookingTicketCinema.ManagementApp.Services
{
    public interface IManagementApiService
    {
        Task<List<MovieViewModel>> GetMoviesAsync();
        Task<MovieViewModel> GetMovieByIdAsync(int id);

        // Dùng ViewModel để gửi dữ liệu [FromForm]
        Task<MovieViewModel> CreateMovieAsync(MovieCreateViewModel model);
        Task UpdateMovieAsync(int id, MovieEditViewModel model);
        Task DeleteMovieAsync(int id);
    }
}
