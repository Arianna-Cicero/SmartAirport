using System.ComponentModel.DataAnnotations;

namespace FlightService.Models
{
    public class FlightSchedule
    {
        [Key]
        public int flightno { get; set; }

        [Required]
        public int from { get; set; }
        public Airport FromAirport { get; set; } // Navigation property

        [Required]
        public int to { get; set; }
        public Airport ToAirport { get; set; } // Navigation property

        [Required]
        public DateTime departure { get; set; }
        [Required]
        public DateTime arrival { get; set; }

        [Required]
        public int airline_id { get; set; }
        public Airline Airline { get; set; } // Navigation property

        [Required]
        public int monday { get; set; }
        [Required]
        public int tuesday { get; set; }
        [Required]
        public int wednesday { get; set; }
        [Required]
        public int thursday { get; set; }
        [Required]
        public int friday { get; set; }
        [Required]
        public int saturday { get; set; }
        [Required]
        public int sunday { get; set; }

    }
}
