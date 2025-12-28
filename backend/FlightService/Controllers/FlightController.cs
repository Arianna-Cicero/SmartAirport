using FlightService.Data;
using FlightService.DTOs;
using FlightService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightController : ControllerBase
    {
        private readonly FlightServiceDbContext _context;
        private readonly ILogger<FlightController> _logger;

        public FlightController(FlightServiceDbContext context, ILogger<FlightController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/flight
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightReadDto>>> GetFlights()
        {
            var flights = await _context.Flights.ToListAsync();
            
            var flightDtos = flights.Select(f => new FlightReadDto
            {
                FlightId = f.flight_id,
                Flightno = f.flightno,
                From = f.from,
                To = f.to,
                Departure = f.departure,
                Arrival = f.arrival,
                AirlineId = f.airline_id,
                AirplaneId = f.airplane_id
            }).ToList();

            return Ok(flightDtos);
        }

        // GET: api/flight/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightReadDto>> GetFlight(int id)
        {
            var flight = await _context.Flights.FindAsync(id);

            if (flight == null)
            {
                return NotFound();
            }

            var flightDto = new FlightReadDto
            {
                FlightId = flight.flight_id,
                Flightno = flight.flightno,
                From = flight.from,
                To = flight.to,
                Departure = flight.departure,
                Arrival = flight.arrival,
                AirlineId = flight.airline_id,
                AirplaneId = flight.airplane_id
            };

            return Ok(flightDto);
        }

        // GET: api/flight/flightno/{flightno}
        [HttpGet("flightno/{flightno}")]
        public async Task<ActionResult<IEnumerable<FlightReadDto>>> GetFlightByFlightNo(string flightno)
        {
            var flights = await _context.Flights
                .Where(f => f.flightno == flightno)
                .ToListAsync();

            if (flights == null || !flights.Any())
            {
                return NotFound();
            }

            var flightDtos = flights.Select(f => new FlightReadDto
            {
                FlightId = f.flight_id,
                Flightno = f.flightno,
                From = f.from,
                To = f.to,
                Departure = f.departure,
                Arrival = f.arrival,
                AirlineId = f.airline_id,
                AirplaneId = f.airplane_id
            }).ToList();

            return Ok(flightDtos);
        }

        // POST: api/flight
        [HttpPost]
        public async Task<ActionResult<FlightReadDto>> CreateFlight(FlightCreateDto flightCreateDto)
        {
            var flight = new Flight
            {
                flightno = flightCreateDto.Flightno,
                from = flightCreateDto.From,
                to = flightCreateDto.To,
                departure = flightCreateDto.Departure,
                arrival = flightCreateDto.Arrival,
                airline_id = flightCreateDto.AirlineId,
                airplane_id = flightCreateDto.AirplaneId
            };

            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            var flightDto = new FlightReadDto
            {
                FlightId = flight.flight_id,
                Flightno = flight.flightno,
                From = flight.from,
                To = flight.to,
                Departure = flight.departure,
                Arrival = flight.arrival,
                AirlineId = flight.airline_id,
                AirplaneId = flight.airplane_id
            };

            return CreatedAtAction(nameof(GetFlight), new { id = flight.flight_id }, flightDto);
        }

        // PUT: api/flight/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFlight(int id, FlightUpdateDto flightUpdateDto)
        {
            var flight = await _context.Flights.FindAsync(id);

            if (flight == null)
            {
                return NotFound();
            }

            flight.flightno = flightUpdateDto.Flightno;
            flight.from = flightUpdateDto.From;
            flight.to = flightUpdateDto.To;
            flight.departure = flightUpdateDto.Departure;
            flight.arrival = flightUpdateDto.Arrival;
            flight.airline_id = flightUpdateDto.AirlineId;
            flight.airplane_id = flightUpdateDto.AirplaneId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await FlightExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/flight/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            var flight = await _context.Flights.FindAsync(id);

            if (flight == null)
            {
                return NotFound();
            }

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> FlightExists(int id)
        {
            return await _context.Flights.AnyAsync(e => e.flight_id == id);
        }
    }
}
