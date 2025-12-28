using System.ComponentModel.DataAnnotations;

namespace AirportAPI.Models
{
    public class Fact_passenger_volume
    {
        [Key]
        public int fact_id { get; set; }
        [Required]
        public int flight_id { get; set; }
        [Required]
        public int airline_id { get; set; }
        [Required]
        public int airplane_id { get; set; }
        public int total_passageiros { get; set; }
        public float preco_total { get; set; }
    }
}
