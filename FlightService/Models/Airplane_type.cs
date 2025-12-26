using System.ComponentModel.DataAnnotations;

namespace AirportAPI.Models
{
    public class Airplane_type
    {
        [Key]
        public int type_id { get; set; }
        [Required]
        [StringLength(50)]
        public string identifier { get; set; }
        public string description { get; set; }
    }
}
