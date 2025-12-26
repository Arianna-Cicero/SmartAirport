namespace SensorService.Models
{
    public class SensorData
    {
        public string AirportCode { get; set; }
        public double Temperature { get; set; }
        public int RunwayOccupancy { get; set; }
        public string RunwayStatus { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
