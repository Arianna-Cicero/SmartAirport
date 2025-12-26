using System.ComponentModel.DataAnnotations;

namespace FlightService.Models
{
    public class Passenger
    {
        [Key]
        public int passenger_id { get; set; }
        [Required]
        [StringLength(100)]
        public string firstname { get; set; }
        [Required]
        [StringLength(100)]
        public string lastname { get; set; }
        [Required]
        [StringLength(9)]
        public string Passportno { get; set; }
    }
}
