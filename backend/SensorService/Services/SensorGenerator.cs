using SensorService.Models;

namespace SensorService.Services
{
    public class SensorGenerator
    {
        private static readonly Random _random = new();

        public SensorData Generate(string airportCode)
        {
            return new SensorData
            {
                AirportCode = airportCode,
                Temperature = Math.Round(_random.NextDouble() * 25 + 5, 1),
                RunwayOccupancy = _random.Next(0, 100),
                RunwayStatus = _random.Next(0, 10) > 1 ? "OPEN" : "CLOSED",
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
