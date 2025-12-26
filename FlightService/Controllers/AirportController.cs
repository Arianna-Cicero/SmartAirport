using FlightService.Data;
using FlightService.DTOs;
using FlightService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AirportController : ControllerBase
    {
        private readonly FlightServiceDbContext _context;
        private readonly ILogger<AirportController> _logger;

        public AirportController(FlightServiceDbContext context, ILogger<AirportController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/airports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AirportReadDto>>> GetAirports()
        {
            var airports = await _context.Airports.ToListAsync();
            
            var airportDtos = airports.Select(a => new AirportReadDto
            {
                AirportId = (short)a.airport_id,
                Iata = a.iata,
                Icao = a.icao,
                Name = a.name
            }).ToList();

            return Ok(airportDtos);
        }

        // GET: api/airports/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AirportReadDto>> GetAirport(short id)
        {
            var airport = await _context.Airports.FindAsync(id);

            if (airport == null)
            {
                return NotFound();
            }

            var airportDto = new AirportReadDto
            {
                AirportId = (short)airport.airport_id,
                Iata = airport.iata,
                Icao = airport.icao,
                Name = airport.name
            };

            return Ok(airportDto);
        }

        // GET: api/airports/iata/{code}
        [HttpGet("iata/{code}")]
        public async Task<ActionResult<AirportReadDto>> GetAirportByIata(string code)
        {
            var airport = await _context.Airports
                .FirstOrDefaultAsync(a => a.iata == code);

            if (airport == null)
            {
                return NotFound();
            }

            var airportDto = new AirportReadDto
            {
                AirportId = (short)airport.airport_id,
                Iata = airport.iata,
                Icao = airport.icao,
                Name = airport.name
            };

            return Ok(airportDto);
        }

        // POST: api/airports
        [HttpPost]
        public async Task<ActionResult<AirportReadDto>> CreateAirport(AirportCreateDto airportCreateDto)
        {
            var airport = new Airport
            {
                iata = airportCreateDto.Iata,
                icao = airportCreateDto.Icao,
                name = airportCreateDto.Name
            };

            _context.Airports.Add(airport);
            await _context.SaveChangesAsync();

            var airportDto = new AirportReadDto
            {
                AirportId = (short)airport.airport_id,
                Iata = airport.iata,
                Icao = airport.icao,
                Name = airport.name
            };

            return CreatedAtAction(nameof(GetAirport), new { id = airport.airport_id }, airportDto);
        }

        // PUT: api/airports/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAirport(short id, AirportUpdateDto airportUpdateDto)
        {
            var airport = await _context.Airports.FindAsync(id);

            if (airport == null)
            {
                return NotFound();
            }

            airport.iata = airportUpdateDto.Iata;
            airport.icao = airportUpdateDto.Icao;
            airport.name = airportUpdateDto.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AirportExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/airports/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAirport(short id)
        {
            var airport = await _context.Airports.FindAsync(id);

            if (airport == null)
            {
                return NotFound();
            }

            _context.Airports.Remove(airport);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> AirportExists(short id)
        {
            return await _context.Airports.AnyAsync(e => e.airport_id == id);
        }
    }
}
