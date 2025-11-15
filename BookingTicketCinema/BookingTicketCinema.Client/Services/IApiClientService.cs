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

        // Lấy thông tin Suất chiếu (từ ShowtimeEndpoints)
        Task<ShowtimeBookingViewModel> GetShowtimeForBookingAsync(int showtimeId);

        // Lấy thông tin Phòng (từ RoomEndpoints)
        Task<RoomViewModel> GetRoomByIdAsync(int roomId); // (Bạn cần tạo RoomViewModel)

        // Lấy sơ đồ ghế (từ SeatEndpoints)
        Task<List<SeatViewModel>> GetSeatsByRoomAsync(int roomId);

        // Lấy loại ghế (từ SeatGroupEndpoints)
        Task<List<SeatGroupViewModel>> GetSeatGroupsByRoomAsync(int roomId);

        // Lấy ghế đã bán (từ TicketController)
        Task<List<int>> GetTakenSeatIdsAsync(int showtimeId);

        // Đặt vé (từ TicketController)
        //Task<BookingResponseViewModel> BookTicketsAsync(BookingRequestViewModel request, string token);
        Task<PaymentResponseDto> CreatePendingPaymentAsync(PaymentRequestDto request, string token);

        // 2. (Mới) Gọi POST /api/payment/{id}/confirm
        Task ConfirmPaymentAsync(int paymentId, string token);

        // 3. (Mới) Gọi PUT /api/payment/{id}/cancel
        Task CancelPaymentAsync(int paymentId, string token);
    }
}
