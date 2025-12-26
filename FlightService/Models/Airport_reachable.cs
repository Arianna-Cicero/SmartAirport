using System.ComponentModel.DataAnnotations;

namespace AirportAPI.Models
{
    public class Airport_reachable
    {
        [Key]
        public int airport_id { get; set; }
        [Required]
        public int hops { get; set; }
    }
}
