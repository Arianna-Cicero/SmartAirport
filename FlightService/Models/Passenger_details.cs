using System.ComponentModel.DataAnnotations;

namespace AirportAPI.Models
{
    public class Passenger_details
    {
        [Key]
        public string passenger_id { get; set; }
        public Passenger Passenger { get; set; } // Navigation property

        [Required]
        public DateTime birthdate{ get; set; }
        [Required]
        [StringLength(1)]
        public string sex { get; set; }
        [StringLength(100)]
        public string street { get; set; }
        [StringLength(100)]
        public string city { get; set; }
        public int zip { get; set; }
        [Required]
        [StringLength(100)]
        public string country { get; set; }
        [Required]
        [StringLength(120)]
        public string email { get; set; }
        [Required]
        [StringLength(100)]
        public string telephone { get; set; }

    }
}
