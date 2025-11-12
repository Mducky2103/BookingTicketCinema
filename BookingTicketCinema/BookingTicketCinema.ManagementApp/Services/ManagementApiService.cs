using System.Net.Http.Headers;
using BookingTicketCinema.ManagementApp.ViewModels;

namespace BookingTicketCinema.ManagementApp.Services
{
    public class ManagementApiService : IManagementApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _apiBaseUrl;

        public ManagementApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _apiBaseUrl = _configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("ApiBaseUrl not configured.");
        }

        // Hàm helper để lấy Client đã xác thực (với Token của Admin/Staff)
        private HttpClient GetAuthorizedClient()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var token = _httpContextAccessor.HttpContext?
                .User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;

            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("User is not authenticated.");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public async Task<List<MovieViewModel>> GetMoviesAsync()
        {
            var client = GetAuthorizedClient();
            return await client.GetFromJsonAsync<List<MovieViewModel>>($"{_apiBaseUrl}/api/movie") ?? new();
        }

        public async Task<MovieViewModel> GetMovieByIdAsync(int id)
        {
            var client = GetAuthorizedClient();
            return await client.GetFromJsonAsync<MovieViewModel>($"{_apiBaseUrl}/api/movie/{id}")
                ?? throw new Exception("Movie not found.");
        }

        public async Task DeleteMovieAsync(int id)
        {
            var client = GetAuthorizedClient();
            var response = await client.DeleteAsync($"{_apiBaseUrl}/api/movie/{id}");
            response.EnsureSuccessStatusCode();
        }

        // --- HÀM TẠO MỚI (DÙNG [FromForm]) ---
        public async Task<MovieViewModel> CreateMovieAsync(MovieCreateViewModel model)
        {
            var client = GetAuthorizedClient();

            // Dùng MultipartFormDataContent thay vì JSON
            using var formData = new MultipartFormDataContent();

            // Thêm các trường dữ liệu (khớp với MovieCreateRequest của API)
            formData.Add(new StringContent(model.Title), nameof(model.Title));
            formData.Add(new StringContent(model.Description ?? ""), nameof(model.Description));
            formData.Add(new StringContent(model.Genre ?? ""), nameof(model.Genre));
            formData.Add(new StringContent(model.TrailerUrl ?? ""), nameof(model.TrailerUrl));
            formData.Add(new StringContent(model.Duration), nameof(model.Duration)); // "02:10:00"
            formData.Add(new StringContent(model.ReleaseDate), nameof(model.ReleaseDate)); // "2025-11-11"
            formData.Add(new StringContent(model.Status.ToString()), nameof(model.Status)); // 0, 1, hoặc 2

            // Thêm file (nếu có)
            if (model.PosterFile != null)
            {
                var fileStream = new StreamContent(model.PosterFile.OpenReadStream());
                fileStream.Headers.ContentType = new MediaTypeHeaderValue(model.PosterFile.ContentType);
                formData.Add(fileStream, nameof(model.PosterFile), model.PosterFile.FileName);
            }

            var response = await client.PostAsync($"{_apiBaseUrl}/api/movie", formData);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MovieViewModel>() ?? new();
        }

        // --- HÀM CẬP NHẬT (DÙNG [FromForm]) ---
        public async Task UpdateMovieAsync(int id, MovieEditViewModel model)
        {
            var client = GetAuthorizedClient();

            using var formData = new MultipartFormDataContent();

            // Thêm các trường (API của bạn cho phép null/trống để không cập nhật)
            formData.Add(new StringContent(model.Title), nameof(model.Title));
            formData.Add(new StringContent(model.Description ?? ""), nameof(model.Description));
            formData.Add(new StringContent(model.Genre ?? ""), nameof(model.Genre));
            formData.Add(new StringContent(model.TrailerUrl ?? ""), nameof(model.TrailerUrl));
            formData.Add(new StringContent(model.Duration), nameof(model.Duration));
            formData.Add(new StringContent(model.ReleaseDate), nameof(model.ReleaseDate));
            formData.Add(new StringContent(model.Status.ToString()), nameof(model.Status));

            // Thêm file MỚI (nếu có)
            if (model.PosterFile != null)
            {
                var fileStream = new StreamContent(model.PosterFile.OpenReadStream());
                fileStream.Headers.ContentType = new MediaTypeHeaderValue(model.PosterFile.ContentType);
                formData.Add(fileStream, nameof(model.PosterFile), model.PosterFile.FileName);
            }

            var response = await client.PutAsync($"{_apiBaseUrl}/api/movie/{id}", formData);
            response.EnsureSuccessStatusCode();
        }
    }
}

