using System.ComponentModel.DataAnnotations;

namespace FlightService.Models
{
    public class AirportGeo
    {
        [Key]
        public int airport_id { get; set; }
        [Required]
        [StringLength(50)]
        public string name { get; set; }
        [Required]
        [StringLength(50)]
        public string city { get; set; }
        [Required]
        [StringLength(50)]
        public string Country { get; set; }
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longitude { get; set; }
    }
}
