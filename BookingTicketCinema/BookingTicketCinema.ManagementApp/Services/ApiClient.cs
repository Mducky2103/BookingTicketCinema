using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BookingTicketCinema.ManagementApp.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;
    private readonly string _baseUrl;

    public ApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5098/api";
        
        // Set default headers
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling GET {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object? data = null)
    {
        try
        {
            var json = data != null ? JsonSerializer.Serialize(data) : null;
            var content = json != null ? new StringContent(json, Encoding.UTF8, "application/json") : null;
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling POST {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object? data = null)
    {
        try
        {
            var json = data != null ? JsonSerializer.Serialize(data) : null;
            var content = json != null ? new StringContent(json, Encoding.UTF8, "application/json") : null;
            
            var response = await _httpClient.PutAsync($"{_baseUrl}/{endpoint}", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling PUT {Endpoint}", endpoint);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling DELETE {Endpoint}", endpoint);
            throw;
        }
    }
}

