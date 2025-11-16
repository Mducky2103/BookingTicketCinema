using BookingTicketCinema.WebApp.ViewModel;

namespace BookingTicketCinema.WebApp.Services
{
    public interface IApiClientService
    {
        Task<List<MovieFeaturedViewModel>> GetFeaturedMoviesAsync();
        Task<List<MovieCardViewModel>> GetNowShowingMoviesAsync();
        Task<List<MovieCardViewModel>> GetComingSoonMoviesAsync();
        Task<HttpResponseMessage> GetAsync(string url);
        Task<HttpResponseMessage> PostAsync(string url, HttpContent content);
        Task<HttpResponseMessage> PutAsync(string url, HttpContent content);
        Task<HttpResponseMessage> DeleteAsync(string url);
        Task<MovieDetailViewModel> GetMovieByIdAsync(int id);
        Task<List<ShowtimeDetailViewModel>> GetShowtimesByMovieAsync(int movieId);

        Task<ShowtimeBookingViewModel> GetShowtimeForBookingAsync(int showtimeId);
        Task<RoomViewModel> GetRoomByIdAsync(int roomId); 
        Task<List<SeatViewModel>> GetSeatsByRoomAsync(int roomId);
        Task<List<SeatGroupViewModel>> GetSeatGroupsByRoomAsync(int roomId);

        // Lấy ghế đã bán (từ TicketController)
        Task<List<int>> GetTakenSeatIdsAsync(int showtimeId);

        // Đặt vé (từ TicketController)
        //Task<BookingResponseViewModel> BookTicketsAsync(BookingRequestViewModel request, string token);
        Task<PaymentResponseDto> CreatePendingPaymentAsync(PaymentRequestDto request, string token);

        Task ConfirmPaymentAsync(int paymentId, string token);

        Task CancelPaymentAsync(int paymentId, string token);

        Task<List<TicketHistoryDto>> GetMyTicketHistoryAsync(string token);
        Task<PaymentResponseDto> GetPaymentSummaryAsync(int paymentId, string token);
        Task<PaymentResponseDto> GetPaymentDetailsAsync(int paymentId, string token);
        Task<List<MovieViewModel>> GetMoviesAsync(string? searchTerm = null);
        Task<PaymentResponseDto> ApplyVoucherAsync(int paymentId, string code, string token);
    }
}
