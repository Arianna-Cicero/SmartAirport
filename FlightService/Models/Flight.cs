using System.ComponentModel.DataAnnotations;

namespace FlightService.Models
{
    public class Flight
    {
        [Key]
        public int flight_id { get; set; }
        [Required]
        [StringLength(8)]
        public string flightno { get; set; }

        [Required]
        public int from { get; set; }
        public Airport FromAirport { get; set; } // Navigation property


        [Required]
        public int to { get; set; }
        public Airport ToAirport { get; set; } // Navigation property

        [Required]
        public TimeOnly departure { get; set; }
        [Required]
        public TimeOnly arrival { get; set; }

        [Required]
        public int airline_id { get; set; }
        public Airline Airline { get; set; } // Navigation property

        [Required]
        public int airplane_id { get; set; }
        public Airplane Airplane { get; set; } // Navigation property

    }
}
