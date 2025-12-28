namespace FlightService.DTOs
{
    public class AirlineReadDto
    {
        public int AirlineId { get; set; }
        public string Iata { get; set; }
        public string Airlinename { get; set; }
        public int BaseAirport { get; set; }
    }
}
