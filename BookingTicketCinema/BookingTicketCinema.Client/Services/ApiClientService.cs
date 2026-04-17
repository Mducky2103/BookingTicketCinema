using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BookingTicketCinema.WebApp.ViewModel;

namespace BookingTicketCinema.WebApp.Services
{
    public class ApiClientService : IApiClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ApiClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<List<MovieFeaturedViewModel>> GetFeaturedMoviesAsync()
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<List<MovieFeaturedViewModel>>("api/MovieForClient/featured") ?? new();
        }

        public async Task<List<MovieCardViewModel>> GetNowShowingMoviesAsync()
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<List<MovieCardViewModel>>("api/MovieForClient/now-showing") ?? new();
        }

        public async Task<List<MovieCardViewModel>> GetComingSoonMoviesAsync()
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<List<MovieCardViewModel>>("api/MovieForClient/coming-soon") ?? new();
        }

        // === Thêm phần Booking ===
        public Task<HttpResponseMessage> GetAsync(string url)
        {
            var client = CreateClient();
            return client.GetAsync(url);
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            var client = CreateClient();
            return client.PostAsync(url, content);
        }

        public Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            var client = CreateClient();
            return client.PutAsync(url, content);
        }

        public Task<HttpResponseMessage> DeleteAsync(string url)
        {
            var client = CreateClient();
            return client.DeleteAsync(url);
        }

        // === Thêm phần Showtime ===
        public async Task<MovieDetailViewModel> GetMovieByIdAsync(int id)
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<MovieDetailViewModel>($"api/Movie/{id}")
                ?? throw new Exception("Không tìm thấy phim.");
        }

        public async Task<List<ShowtimeDetailViewModel>> GetShowtimesByMovieAsync(int movieId)
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<List<ShowtimeDetailViewModel>>($"api/showtimes/GetShowtimesByMovie/{movieId}") ?? new();
        }

        public async Task<ShowtimeBookingViewModel> GetShowtimeForBookingAsync(int showtimeId)
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<ShowtimeBookingViewModel>($"api/showtimes/GetShowtimeById/{showtimeId}")
                ?? throw new Exception("Không tìm thấy suất chiếu.");
        }

        public async Task<RoomViewModel> GetRoomByIdAsync(int roomId)
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<RoomViewModel>($"api/rooms/{roomId}")
                ?? throw new Exception("Không tìm thấy phòng.");
        }

        public async Task<List<SeatViewModel>> GetSeatsByRoomAsync(int roomId)
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<List<SeatViewModel>>($"api/seats/room/{roomId}") ?? new();
        }

        public async Task<List<SeatGroupViewModel>> GetSeatGroupsByRoomAsync(int roomId)
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<List<SeatGroupViewModel>>($"api/seatgroups/room/{roomId}") ?? new();
        }

        public async Task<List<int>> GetTakenSeatIdsAsync(int showtimeId)
        {
            var client = CreateClient();
            return await client.GetFromJsonAsync<List<int>>($"api/ticket/showtime/{showtimeId}/taken-seats") ?? new();
        }

        //public async Task<BookingResponseViewModel> BookTicketsAsync(BookingRequestViewModel request, string token)
        //{
        //    var client = CreateClient();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    var response = await client.PostAsJsonAsync("api/ticket/book", request);
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var error = await response.Content.ReadFromJsonAsync<object>();
        //        throw new Exception($"Đặt vé thất bại: {error?.ToString()}");
        //    }
        //    return await response.Content.ReadFromJsonAsync<BookingResponseViewModel>() ?? new();
        //}
        public async Task<PaymentResponseDto> CreatePendingPaymentAsync(PaymentRequestDto request, string token)
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync("api/payment/create", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<object>();
                throw new Exception($"Lỗi tạo đơn hàng: {error?.ToString()}");
            }
            return await response.Content.ReadFromJsonAsync<PaymentResponseDto>()
                ?? throw new Exception("Không nhận được phản hồi Payment.");
        }

        public async Task ConfirmPaymentAsync(int paymentId, string token)
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"api/payment/{paymentId}/confirm", null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<object>();
                throw new Exception($"Lỗi xác nhận: {error?.ToString()}");
            }
        }

        public async Task CancelPaymentAsync(int paymentId, string token)
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PutAsync($"api/payment/{paymentId}/cancel", null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<object>();
                throw new Exception($"Lỗi hủy vé: {error?.ToString()}");
            }
        }

        public async Task<List<TicketHistoryDto>> GetMyTicketHistoryAsync(string token)
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Gọi API trong TicketController
            return await client.GetFromJsonAsync<List<TicketHistoryDto>>("api/ticket/my-history") ?? new();
        }

        public async Task<PaymentResponseDto> GetPaymentSummaryAsync(int paymentId, string token)
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Gọi API mới trong PaymentController
            return await client.GetFromJsonAsync<PaymentResponseDto>($"api/payment/summary/{paymentId}")
                ?? throw new Exception("Không tìm thấy đơn hàng.");
        }
        public async Task<PaymentResponseDto> GetPaymentDetailsAsync(int paymentId, string token)
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gọi API mới
            var response = await client.GetAsync($"api/payment/details/{paymentId}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<object>();
                throw new Exception($"Lỗi tải chi tiết vé: {error?.ToString()}");
            }
            // Dùng lại DTO "PaymentResponseDto" (đã có)
            return await response.Content.ReadFromJsonAsync<PaymentResponseDto>()
                ?? throw new Exception("Không nhận được phản hồi chi tiết vé.");
        }
        public async Task<List<MovieViewModel>> GetMoviesAsync(string? searchTerm = null)
        {
            var client = CreateClient();

            string endpoint = "api/MovieForClient";
            if (!string.IsNullOrEmpty(searchTerm))
            {
                endpoint += $"?search={Uri.EscapeDataString(searchTerm)}";
            }

            return await client.GetFromJsonAsync<List<MovieViewModel>>(endpoint) ?? new();
        }

        public async Task<PaymentResponseDto> ApplyVoucherAsync(int paymentId, string code, string token)
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new { PaymentId = paymentId, Code = code };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/payment/apply-voucher", content);

            // Xử lý lỗi (quan trọng)
            if (!response.IsSuccessStatusCode)
            {
                // Đọc lỗi từ API (VD: "Mã đã dùng rồi")
                var errorResponse = await response.Content.ReadFromJsonAsync<object>();
                throw new Exception(errorResponse?.ToString() ?? "Lỗi không xác định");
            }

            // Trả về Payment DTO mới
            return await response.Content.ReadFromJsonAsync<PaymentResponseDto>()
                ?? throw new Exception("Không nhận được phản hồi sau khi áp dụng mã.");
        }
    }
}
