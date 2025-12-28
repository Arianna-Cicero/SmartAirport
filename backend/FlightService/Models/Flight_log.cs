using System.ComponentModel.DataAnnotations;

namespace FlightService.Models
{
    public class Flight_log
    {
        [Key]
        public int flight_id { get; set; }
        public TimeOnly log_date { get; set; }
        [StringLength(100)]
        public string user { get; set; }
        [StringLength(8)]
        public string flightno_old { get; set; }
        [StringLength(8)]
        public string flightno_new { get; set; }
        public int from_old { get; set; }
        public int from_new { get; set; }
        public int to_old { get; set; }
        public int to_new { get; set; }
        public TimeOnly departure_old { get; set; }
        public TimeOnly departure_new { get; set; }
        public TimeOnly arrival_old { get; set; }
        public TimeOnly arrival_new { get; set; }
        public int airline_id_old { get; set; }
        public int airline_id_new { get; set; }
        public int airplane_id_old { get; set; }
        public int airplane_id_new { get; set; }
        [StringLength(200)]
        public string comment { get; set; }
    }
}
