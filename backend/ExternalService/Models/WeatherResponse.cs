namespace ExternalService.Models
{
    public class WeatherResponse
    {
        public string AirportCode { get; set; }
        public string Description { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
