namespace FlightService.DTOs
{
    public class AirportReadDto
    {
        public int AirportId { get; set; }
        public string Iata { get; set; }
        public string Icao { get; set; }
        public string Name { get; set; }
    }
}
