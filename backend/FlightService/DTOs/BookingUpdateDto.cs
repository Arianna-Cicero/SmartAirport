namespace FlightService.DTOs
{
    public class BookingUpdateDto
    {
        public int FlightId { get; set; }
        public int PassengerId { get; set; }
        public string Seat { get; set; }
        public float Price { get; set; }
    }
}
