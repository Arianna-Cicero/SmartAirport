using FlightService.Data;
using FlightService.DTOs;
using FlightService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PassengerController : ControllerBase
    {
        private readonly FlightServiceDbContext _context;
        private readonly ILogger<PassengerController> _logger;

        public PassengerController(FlightServiceDbContext context, ILogger<PassengerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/passenger
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PassengerReadDto>>> GetPassengers()
        {
            var passengers = await _context.Passengers.ToListAsync();
            
            var passengerDtos = passengers.Select(p => new PassengerReadDto
            {
                PassengerId = p.passenger_id,
                Firstname = p.firstname,
                Lastname = p.lastname,
                Passportno = p.Passportno
            }).ToList();

            return Ok(passengerDtos);
        }

        // GET: api/passenger/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PassengerReadDto>> GetPassenger(int id)
        {
            var passenger = await _context.Passengers.FindAsync(id);

            if (passenger == null)
            {
                return NotFound();
            }

            var passengerDto = new PassengerReadDto
            {
                PassengerId = passenger.passenger_id,
                Firstname = passenger.firstname,
                Lastname = passenger.lastname,
                Passportno = passenger.Passportno
            };

            return Ok(passengerDto);
        }

        // GET: api/passenger/passport/{passportno}
        [HttpGet("passport/{passportno}")]
        public async Task<ActionResult<PassengerReadDto>> GetPassengerByPassport(string passportno)
        {
            var passenger = await _context.Passengers
                .FirstOrDefaultAsync(p => p.Passportno == passportno);

            if (passenger == null)
            {
                return NotFound();
            }

            var passengerDto = new PassengerReadDto
            {
                PassengerId = passenger.passenger_id,
                Firstname = passenger.firstname,
                Lastname = passenger.lastname,
                Passportno = passenger.Passportno
            };

            return Ok(passengerDto);
        }

        // GET: api/passenger/search?lastname={lastname}
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PassengerReadDto>>> SearchPassengersByLastname([FromQuery] string lastname)
        {
            if (string.IsNullOrWhiteSpace(lastname))
            {
                return BadRequest("Lastname parameter is required");
            }

            var passengers = await _context.Passengers
                .Where(p => p.lastname.Contains(lastname))
                .ToListAsync();

            if (passengers == null || !passengers.Any())
            {
                return NotFound();
            }

            var passengerDtos = passengers.Select(p => new PassengerReadDto
            {
                PassengerId = p.passenger_id,
                Firstname = p.firstname,
                Lastname = p.lastname,
                Passportno = p.Passportno
            }).ToList();

            return Ok(passengerDtos);
        }

        // POST: api/passenger
        [HttpPost]
        public async Task<ActionResult<PassengerReadDto>> CreatePassenger(PassengerCreateDto passengerCreateDto)
        {
            var passenger = new Passenger
            {
                firstname = passengerCreateDto.Firstname,
                lastname = passengerCreateDto.Lastname,
                Passportno = passengerCreateDto.Passportno
            };

            _context.Passengers.Add(passenger);
            await _context.SaveChangesAsync();

            var passengerDto = new PassengerReadDto
            {
                PassengerId = passenger.passenger_id,
                Firstname = passenger.firstname,
                Lastname = passenger.lastname,
                Passportno = passenger.Passportno
            };

            return CreatedAtAction(nameof(GetPassenger), new { id = passenger.passenger_id }, passengerDto);
        }

        // PUT: api/passenger/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePassenger(int id, PassengerUpdateDto passengerUpdateDto)
        {
            var passenger = await _context.Passengers.FindAsync(id);

            if (passenger == null)
            {
                return NotFound();
            }

            passenger.firstname = passengerUpdateDto.Firstname;
            passenger.lastname = passengerUpdateDto.Lastname;
            passenger.Passportno = passengerUpdateDto.Passportno;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PassengerExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/passenger/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePassenger(int id)
        {
            var passenger = await _context.Passengers.FindAsync(id);

            if (passenger == null)
            {
                return NotFound();
            }

            _context.Passengers.Remove(passenger);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> PassengerExists(int id)
        {
            return await _context.Passengers.AnyAsync(e => e.passenger_id == id);
        }
    }
}
