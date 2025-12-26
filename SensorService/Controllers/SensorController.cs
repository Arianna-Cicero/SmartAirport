using Microsoft.AspNetCore.Mvc;
using SensorService.Services;

namespace SensorService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly SensorGenerator _generator;

        public SensorController(SensorGenerator generator)
        {
            _generator = generator;
        }

        // GET: api/sensor/{airportCode}
        [HttpGet("{airportCode}")]
        public IActionResult GetSensorData(string airportCode)
        {
            var data = _generator.Generate(airportCode);
            return Ok(data);
        }
    }
}
