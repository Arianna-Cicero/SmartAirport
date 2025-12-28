using ExternalService.Models;
using System.Text.Json;

namespace ExternalService.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public WeatherService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<WeatherResponse?> GetWeatherAsync(string airportCode)
        {
            // Exemplo simples: usar o código como query
            var baseUrl = _config["WeatherApi:BaseUrl"];
            var apiKey = _config["WeatherApi:ApiKey"];

            var url = $"{baseUrl}?q={airportCode}&appid={apiKey}&units=metric";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            using var stream = await response.Content.ReadAsStreamAsync();
            using var json = await JsonDocument.ParseAsync(stream);

            var root = json.RootElement;

            return new WeatherResponse
            {
                AirportCode = airportCode,
                Description = root.GetProperty("weather")[0].GetProperty("description").GetString(),
                Temperature = root.GetProperty("main").GetProperty("temp").GetDouble(),
                Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32(),
                WindSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble(),
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
