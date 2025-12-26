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
        [Required]
        public int to { get; set; }
        [Required]
        public TimeOnly departure { get; set; }
        [Required]
        public TimeOnly arrival { get; set; }
        [Required]
        public int airline_id { get; set; }
        [Required]
        public int airplane_id { get; set; }
    }
}
