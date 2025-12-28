using System.ComponentModel.DataAnnotations;

namespace FlightService.Models
{
    public class Passenger
    {
        [Key]
        public int passenger_id { get; set; }
        public Passenger_details PassengerDetails { get; set; } // Navigation property

        [Required]
        [StringLength(100)]
        public string firstname { get; set; }
        [Required]
        [StringLength(100)]
        public string lastname { get; set; }
        [Required]
        [StringLength(9)]
        public string Passportno { get; set; }

        public ICollection<Booking> Bookings { get; set; } // Navigation property
    }
}
