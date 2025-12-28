using System.ComponentModel.DataAnnotations;

namespace FlightService.Models
{
    public class Employee
    {
        [Key]
        public int employee_id { get; set; }
        [Required]
        [StringLength(100)]
        public string firstname { get; set; }
        [Required]
        [StringLength(100)]
        public string lastname { get; set; }
        [Required]
        public DateTime birthdate { get; set; }
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
        [StringLength(30)]
        public string telephone { get; set; }
        [StringLength(30)]
        [Required]
        public float salary { get; set; }
        [Required]
        [StringLength(250)]
        public string department { get; set; }
        [Required]
        [StringLength(20)]
        public string username { get; set; }
        [Required]
        [StringLength(32)]
        public string password { get; set; }
    }
}
