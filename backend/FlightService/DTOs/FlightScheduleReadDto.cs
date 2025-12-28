namespace FlightService.DTOs
{
    public class FlightScheduleReadDto
    {
        public int Flightno { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public DateTime Departure { get; set; }
        public DateTime Arrival { get; set; }
        public int AirlineId { get; set; }
        public int Monday { get; set; }
        public int Tuesday { get; set; }
        public int Wednesday { get; set; }
        public int Thursday { get; set; }
        public int Friday { get; set; }
        public int Saturday { get; set; }
        public int Sunday { get; set; }
    }
}
