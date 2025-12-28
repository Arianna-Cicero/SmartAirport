using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FlightService.Models
{
    public class Airline
    {
        [Key]
        [Required]
        public int airline_id { get; set; }
        [Required]
        [StringLength(3)]
        public string iata { get; set; }
        [Required]
        [StringLength(30)]
        public string airlinename { get; set; }
        [Required]
        public int base_airport { get; set; }
        public Airport BaseAirport { get; set; }

        public ICollection<Flight> Flights { get; set; }
        public ICollection<Airplane> Airplanes { get; set; }
    }
}
