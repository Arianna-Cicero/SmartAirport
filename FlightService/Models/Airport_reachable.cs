using System.ComponentModel.DataAnnotations;

namespace FlightService.Models
{
    public class Airport_reachable
    {
        [Key]
        public int airport_id { get; set; }
        [Required]
        public int hops { get; set; }
    }
}
