using System.ComponentModel.DataAnnotations;

namespace FlightService.Models
{
    public class Airport
    {
        [Key]
        public int airport_id { get; set; }
        [Required]
        [StringLength(3)]
        public string iata { get; set; }
        [Required]
        [StringLength(4)]
        public string icao { get; set; }
        [Required]  
        [StringLength(50)]
        public string name { get; set; }
    }
}
