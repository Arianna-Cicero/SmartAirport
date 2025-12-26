using ExternalService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExternalService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public ExternalController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        // GET: api/external/weather/{airportCode}
        [HttpGet("weather/{airportCode}")]
        public async Task<IActionResult> GetWeather(string airportCode)
        {
            var weather = await _weatherService.GetWeatherAsync(airportCode);

            if (weather == null)
                return NotFound("Weather data not available");

            return Ok(weather);
        }
    }
}
