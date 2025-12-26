using FlightService.Data;
using FlightService.DTOs;
using FlightService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AirplaneController : ControllerBase
    {
        private readonly FlightServiceDbContext _context;
        private readonly ILogger<AirplaneController> _logger;

        public AirplaneController(FlightServiceDbContext context, ILogger<AirplaneController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/airplane
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AirplaneReadDto>>> GetAirplanes()
        {
            var airplanes = await _context.Airplanes.ToListAsync();
            
            var airplaneDtos = airplanes.Select(a => new AirplaneReadDto
            {
                AirplaneId = a.airplane_id,
                Capacity = a.capacity,
                TypeId = a.type_id,
                AirplineId = a.airpline_id
            }).ToList();

            return Ok(airplaneDtos);
        }

        // GET: api/airplane/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AirplaneReadDto>> GetAirplane(int id)
        {
            var airplane = await _context.Airplanes.FindAsync(id);

            if (airplane == null)
            {
                return NotFound();
            }

            var airplaneDto = new AirplaneReadDto
            {
                AirplaneId = airplane.airplane_id,
                Capacity = airplane.capacity,
                TypeId = airplane.type_id,
                AirplineId = airplane.airpline_id
            };

            return Ok(airplaneDto);
        }

        // GET: api/airplane/type/{typeId}
        [HttpGet("type/{typeId}")]
        public async Task<ActionResult<IEnumerable<AirplaneReadDto>>> GetAirplanesByType(int typeId)
        {
            var airplanes = await _context.Airplanes
                .Where(a => a.type_id == typeId)
                .ToListAsync();

            if (airplanes == null || !airplanes.Any())
            {
                return NotFound();
            }

            var airplaneDtos = airplanes.Select(a => new AirplaneReadDto
            {
                AirplaneId = a.airplane_id,
                Capacity = a.capacity,
                TypeId = a.type_id,
                AirplineId = a.airpline_id
            }).ToList();

            return Ok(airplaneDtos);
        }

        // GET: api/airplane/airline/{airlineId}
        [HttpGet("airline/{airlineId}")]
        public async Task<ActionResult<IEnumerable<AirplaneReadDto>>> GetAirplanesByAirline(int airlineId)
        {
            var airplanes = await _context.Airplanes
                .Where(a => a.airpline_id == airlineId)
                .ToListAsync();

            if (airplanes == null || !airplanes.Any())
            {
                return NotFound();
            }

            var airplaneDtos = airplanes.Select(a => new AirplaneReadDto
            {
                AirplaneId = a.airplane_id,
                Capacity = a.capacity,
                TypeId = a.type_id,
                AirplineId = a.airpline_id
            }).ToList();

            return Ok(airplaneDtos);
        }

        // POST: api/airplane
        [HttpPost]
        public async Task<ActionResult<AirplaneReadDto>> CreateAirplane(AirplaneCreateDto airplaneCreateDto)
        {
            var airplane = new Airplane
            {
                capacity = airplaneCreateDto.Capacity,
                type_id = airplaneCreateDto.TypeId,
                airpline_id = airplaneCreateDto.AirplineId
            };

            _context.Airplanes.Add(airplane);
            await _context.SaveChangesAsync();

            var airplaneDto = new AirplaneReadDto
            {
                AirplaneId = airplane.airplane_id,
                Capacity = airplane.capacity,
                TypeId = airplane.type_id,
                AirplineId = airplane.airpline_id
            };

            return CreatedAtAction(nameof(GetAirplane), new { id = airplane.airplane_id }, airplaneDto);
        }

        // PUT: api/airplane/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAirplane(int id, AirplaneUpdateDto airplaneUpdateDto)
        {
            var airplane = await _context.Airplanes.FindAsync(id);

            if (airplane == null)
            {
                return NotFound();
            }

            airplane.capacity = airplaneUpdateDto.Capacity;
            airplane.type_id = airplaneUpdateDto.TypeId;
            airplane.airpline_id = airplaneUpdateDto.AirplineId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AirplaneExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/airplane/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAirplane(int id)
        {
            var airplane = await _context.Airplanes.FindAsync(id);

            if (airplane == null)
            {
                return NotFound();
            }

            _context.Airplanes.Remove(airplane);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> AirplaneExists(int id)
        {
            return await _context.Airplanes.AnyAsync(e => e.airplane_id == id);
        }
    }
}
