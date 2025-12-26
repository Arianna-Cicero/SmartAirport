using FlightService.Data;
using FlightService.DTOs;
using FlightService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightScheduleController : ControllerBase
    {
        private readonly FlightServiceDbContext _context;
        private readonly ILogger<FlightScheduleController> _logger;

        public FlightScheduleController(FlightServiceDbContext context, ILogger<FlightScheduleController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/flightschedule
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightScheduleReadDto>>> GetFlightSchedules()
        {
            var schedules = await _context.FlightSchedules.ToListAsync();
            
            var scheduleDtos = schedules.Select(s => new FlightScheduleReadDto
            {
                Flightno = s.flightno,
                From = s.from,
                To = s.to,
                Departure = s.departure,
                Arrival = s.arrival,
                AirlineId = s.airline_id,
                Monday = s.monday,
                Tuesday = s.tuesday,
                Wednesday = s.wednesday,
                Thursday = s.thursday,
                Friday = s.friday,
                Saturday = s.saturday,
                Sunday = s.sunday
            }).ToList();

            return Ok(scheduleDtos);
        }

        // GET: api/flightschedule/{flightno}
        [HttpGet("{flightno}")]
        public async Task<ActionResult<FlightScheduleReadDto>> GetFlightSchedule(int flightno)
        {
            var schedule = await _context.FlightSchedules.FindAsync(flightno);

            if (schedule == null)
            {
                return NotFound();
            }

            var scheduleDto = new FlightScheduleReadDto
            {
                Flightno = schedule.flightno,
                From = schedule.from,
                To = schedule.to,
                Departure = schedule.departure,
                Arrival = schedule.arrival,
                AirlineId = schedule.airline_id,
                Monday = schedule.monday,
                Tuesday = schedule.tuesday,
                Wednesday = schedule.wednesday,
                Thursday = schedule.thursday,
                Friday = schedule.friday,
                Saturday = schedule.saturday,
                Sunday = schedule.sunday
            };

            return Ok(scheduleDto);
        }

        // GET: api/flightschedule/airline/{airlineId}
        [HttpGet("airline/{airlineId}")]
        public async Task<ActionResult<IEnumerable<FlightScheduleReadDto>>> GetFlightSchedulesByAirline(int airlineId)
        {
            var schedules = await _context.FlightSchedules
                .Where(s => s.airline_id == airlineId)
                .ToListAsync();

            if (schedules == null || !schedules.Any())
            {
                return NotFound();
            }

            var scheduleDtos = schedules.Select(s => new FlightScheduleReadDto
            {
                Flightno = s.flightno,
                From = s.from,
                To = s.to,
                Departure = s.departure,
                Arrival = s.arrival,
                AirlineId = s.airline_id,
                Monday = s.monday,
                Tuesday = s.tuesday,
                Wednesday = s.wednesday,
                Thursday = s.thursday,
                Friday = s.friday,
                Saturday = s.saturday,
                Sunday = s.sunday
            }).ToList();

            return Ok(scheduleDtos);
        }

        // GET: api/flightschedule/dayofweek/{day}
        [HttpGet("dayofweek/{day}")]
        public async Task<ActionResult<IEnumerable<FlightScheduleReadDto>>> GetFlightSchedulesByDayOfWeek(string day)
        {
            var schedules = await _context.FlightSchedules.ToListAsync();

            schedules = day.ToLower() switch
            {
                "monday" => schedules.Where(s => s.monday == 1).ToList(),
                "tuesday" => schedules.Where(s => s.tuesday == 1).ToList(),
                "wednesday" => schedules.Where(s => s.wednesday == 1).ToList(),
                "thursday" => schedules.Where(s => s.thursday == 1).ToList(),
                "friday" => schedules.Where(s => s.friday == 1).ToList(),
                "saturday" => schedules.Where(s => s.saturday == 1).ToList(),
                "sunday" => schedules.Where(s => s.sunday == 1).ToList(),
                _ => new List<FlightSchedule>()
            };

            if (!schedules.Any())
            {
                return NotFound();
            }

            var scheduleDtos = schedules.Select(s => new FlightScheduleReadDto
            {
                Flightno = s.flightno,
                From = s.from,
                To = s.to,
                Departure = s.departure,
                Arrival = s.arrival,
                AirlineId = s.airline_id,
                Monday = s.monday,
                Tuesday = s.tuesday,
                Wednesday = s.wednesday,
                Thursday = s.thursday,
                Friday = s.friday,
                Saturday = s.saturday,
                Sunday = s.sunday
            }).ToList();

            return Ok(scheduleDtos);
        }

        // POST: api/flightschedule
        [HttpPost]
        public async Task<ActionResult<FlightScheduleReadDto>> CreateFlightSchedule(FlightScheduleCreateDto scheduleCreateDto)
        {
            var schedule = new FlightSchedule
            {
                flightno = scheduleCreateDto.Flightno,
                from = scheduleCreateDto.From,
                to = scheduleCreateDto.To,
                departure = scheduleCreateDto.Departure,
                arrival = scheduleCreateDto.Arrival,
                airline_id = scheduleCreateDto.AirlineId,
                monday = scheduleCreateDto.Monday,
                tuesday = scheduleCreateDto.Tuesday,
                wednesday = scheduleCreateDto.Wednesday,
                thursday = scheduleCreateDto.Thursday,
                friday = scheduleCreateDto.Friday,
                saturday = scheduleCreateDto.Saturday,
                sunday = scheduleCreateDto.Sunday
            };

            _context.FlightSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            var scheduleDto = new FlightScheduleReadDto
            {
                Flightno = schedule.flightno,
                From = schedule.from,
                To = schedule.to,
                Departure = schedule.departure,
                Arrival = schedule.arrival,
                AirlineId = schedule.airline_id,
                Monday = schedule.monday,
                Tuesday = schedule.tuesday,
                Wednesday = schedule.wednesday,
                Thursday = schedule.thursday,
                Friday = schedule.friday,
                Saturday = schedule.saturday,
                Sunday = schedule.sunday
            };

            return CreatedAtAction(nameof(GetFlightSchedule), new { flightno = schedule.flightno }, scheduleDto);
        }

        // PUT: api/flightschedule/{flightno}
        [HttpPut("{flightno}")]
        public async Task<IActionResult> UpdateFlightSchedule(int flightno, FlightScheduleUpdateDto scheduleUpdateDto)
        {
            var schedule = await _context.FlightSchedules.FindAsync(flightno);

            if (schedule == null)
            {
                return NotFound();
            }

            schedule.from = scheduleUpdateDto.From;
            schedule.to = scheduleUpdateDto.To;
            schedule.departure = scheduleUpdateDto.Departure;
            schedule.arrival = scheduleUpdateDto.Arrival;
            schedule.airline_id = scheduleUpdateDto.AirlineId;
            schedule.monday = scheduleUpdateDto.Monday;
            schedule.tuesday = scheduleUpdateDto.Tuesday;
            schedule.wednesday = scheduleUpdateDto.Wednesday;
            schedule.thursday = scheduleUpdateDto.Thursday;
            schedule.friday = scheduleUpdateDto.Friday;
            schedule.saturday = scheduleUpdateDto.Saturday;
            schedule.sunday = scheduleUpdateDto.Sunday;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await FlightScheduleExists(flightno))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/flightschedule/{flightno}
        [HttpDelete("{flightno}")]
        public async Task<IActionResult> DeleteFlightSchedule(int flightno)
        {
            var schedule = await _context.FlightSchedules.FindAsync(flightno);

            if (schedule == null)
            {
                return NotFound();
            }

            _context.FlightSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> FlightScheduleExists(int flightno)
        {
            return await _context.FlightSchedules.AnyAsync(e => e.flightno == flightno);
        }
    }
}
