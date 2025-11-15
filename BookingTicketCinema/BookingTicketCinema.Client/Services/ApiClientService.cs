using System.Net.Http.Headers;
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
            // Gọi endpoint mới
            return await client.GetFromJsonAsync<List<MovieFeaturedViewModel>>("api/MovieForClient/featured") ?? new();
        }

        public async Task<List<MovieCardViewModel>> GetNowShowingMoviesAsync()
        {
            var client = CreateClient();
            // Gọi endpoint mới
            return await client.GetFromJsonAsync<List<MovieCardViewModel>>("api/MovieForClient/now-showing") ?? new();
        }

        public async Task<List<MovieCardViewModel>> GetComingSoonMoviesAsync()
        {
            var client = CreateClient();
            // Gọi endpoint mới
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
            // Gọi API /seatgroups/room/{id} của bạn
            return await client.GetFromJsonAsync<List<SeatGroupViewModel>>($"api/seatgroups/room/{roomId}") ?? new();
        }

        public async Task<List<int>> GetTakenSeatIdsAsync(int showtimeId)
        {
            var client = CreateClient();
            // Gọi API mới trong TicketController
            return await client.GetFromJsonAsync<List<int>>($"api/ticket/showtime/{showtimeId}/taken-seats") ?? new();
        }

        // (Code hàm BookTicketsAsync vẫn giữ nguyên)
        public async Task<BookingResponseViewModel> BookTicketsAsync(BookingRequestViewModel request, string token)
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsJsonAsync("api/ticket/book", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<object>();
                throw new Exception($"Đặt vé thất bại: {error?.ToString()}");
            }
            return await response.Content.ReadFromJsonAsync<BookingResponseViewModel>() ?? new();
        }
    }
}
