namespace FlightService.DTOs
{
    public class FlightReadDto
    {
        public int FlightId { get; set; }
        public string Flightno { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public TimeOnly Departure { get; set; }
        public TimeOnly Arrival { get; set; }
        public int AirlineId { get; set; }
        public int AirplaneId { get; set; }
    }
}
