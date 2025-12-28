using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightService.Models
{
    public class Airplane
    {
        [Key]
        public int airplane_id { get; set; }
        [Required]
        public int capacity { get; set; }
        [Required]
        public int type_id { get; set; }
        public Airplane_type AirplaneType { get; set; }
        [Required]
        public int airline_id { get; set; }
        public Airline Airline { get; set; }

    }
}
