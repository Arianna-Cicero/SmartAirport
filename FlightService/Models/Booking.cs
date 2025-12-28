using System.ComponentModel.DataAnnotations;

namespace FlightService.Models
{
    public class Booking
    {
        [Key]
        public int booking_id { get; set; }

        [Required]
        public int flight_id { get; set; }
        public Flight Flight { get; set; }

        [Required]
        public int passenger_id { get; set; }
        public Passenger Passenger { get; set; }

        [Required]
        [StringLength(4)]
        public string seat { get; set; }
        [Required]
        public float price { get; set; }
    }
}
