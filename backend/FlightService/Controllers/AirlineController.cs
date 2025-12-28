using FlightService.Data;
using FlightService.DTOs;
using FlightService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AirlineController : ControllerBase
    {
        private readonly FlightServiceDbContext _context;
        private readonly ILogger<AirlineController> _logger;

        public AirlineController(FlightServiceDbContext context, ILogger<AirlineController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/airline
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AirlineReadDto>>> GetAirlines()
        {
            var airlines = await _context.Airlines.ToListAsync();
            
            var airlineDtos = airlines.Select(a => new AirlineReadDto
            {
                AirlineId = a.airline_id,
                Iata = a.iata,
                Airlinename = a.airlinename,
                BaseAirport = a.base_airport
            }).ToList();

            return Ok(airlineDtos);
        }

        // GET: api/airline/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AirlineReadDto>> GetAirline(int id)
        {
            var airline = await _context.Airlines.FindAsync(id);

            if (airline == null)
            {
                return NotFound();
            }

            var airlineDto = new AirlineReadDto
            {
                AirlineId = airline.airline_id,
                Iata = airline.iata,
                Airlinename = airline.airlinename,
                BaseAirport = airline.base_airport
            };

            return Ok(airlineDto);
        }

        // GET: api/airline/iata/{iata}
        [HttpGet("iata/{iata}")]
        public async Task<ActionResult<AirlineReadDto>> GetAirlineByIata(string iata)
        {
            var airline = await _context.Airlines
                .FirstOrDefaultAsync(a => a.iata == iata);

            if (airline == null)
            {
                return NotFound();
            }

            var airlineDto = new AirlineReadDto
            {
                AirlineId = airline.airline_id,
                Iata = airline.iata,
                Airlinename = airline.airlinename,
                BaseAirport = airline.base_airport
            };

            return Ok(airlineDto);
        }

        // GET: api/airline/baseairport/{airportId}
        [HttpGet("baseairport/{airportId}")]
        public async Task<ActionResult<IEnumerable<AirlineReadDto>>> GetAirlinesByBaseAirport(int airportId)
        {
            var airlines = await _context.Airlines
                .Where(a => a.base_airport == airportId)
                .ToListAsync();

            if (airlines == null || !airlines.Any())
            {
                return NotFound();
            }

            var airlineDtos = airlines.Select(a => new AirlineReadDto
            {
                AirlineId = a.airline_id,
                Iata = a.iata,
                Airlinename = a.airlinename,
                BaseAirport = a.base_airport
            }).ToList();

            return Ok(airlineDtos);
        }

        // POST: api/airline
        [HttpPost]
        public async Task<ActionResult<AirlineReadDto>> CreateAirline(AirlineCreateDto airlineCreateDto)
        {
            var airline = new Airline
            {
                airline_id = airlineCreateDto.AirlineId,
                iata = airlineCreateDto.Iata,
                airlinename = airlineCreateDto.Airlinename,
                base_airport = airlineCreateDto.BaseAirport
            };

            _context.Airlines.Add(airline);
            await _context.SaveChangesAsync();

            var airlineDto = new AirlineReadDto
            {
                AirlineId = airline.airline_id,
                Iata = airline.iata,
                Airlinename = airline.airlinename,
                BaseAirport = airline.base_airport
            };

            return CreatedAtAction(nameof(GetAirline), new { id = airline.airline_id }, airlineDto);
        }

        // PUT: api/airline/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAirline(int id, AirlineUpdateDto airlineUpdateDto)
        {
            var airline = await _context.Airlines.FindAsync(id);

            if (airline == null)
            {
                return NotFound();
            }

            airline.iata = airlineUpdateDto.Iata;
            airline.airlinename = airlineUpdateDto.Airlinename;
            airline.base_airport = airlineUpdateDto.BaseAirport;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AirlineExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/airline/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAirline(int id)
        {
            var airline = await _context.Airlines.FindAsync(id);

            if (airline == null)
            {
                return NotFound();
            }

            _context.Airlines.Remove(airline);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> AirlineExists(int id)
        {
            return await _context.Airlines.AnyAsync(e => e.airline_id == id);
        }
    }
}
