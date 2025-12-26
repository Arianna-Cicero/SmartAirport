using FlightService.Data;
using FlightService.DTOs;
using FlightService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly FlightServiceDbContext _context;
        private readonly ILogger<BookingController> _logger;

        public BookingController(FlightServiceDbContext context, ILogger<BookingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/booking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingReadDto>>> GetBookings()
        {
            var bookings = await _context.Bookings.ToListAsync();
            
            var bookingDtos = bookings.Select(b => new BookingReadDto
            {
                BookingId = b.booking_id,
                FlightId = b.flight_id,
                PassengerId = b.passenger_id,
                Seat = b.seat,
                Price = b.price
            }).ToList();

            return Ok(bookingDtos);
        }

        // GET: api/booking/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingReadDto>> GetBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            var bookingDto = new BookingReadDto
            {
                BookingId = booking.booking_id,
                FlightId = booking.flight_id,
                PassengerId = booking.passenger_id,
                Seat = booking.seat,
                Price = booking.price
            };

            return Ok(bookingDto);
        }

        // GET: api/booking/flight/{flightId}
        [HttpGet("flight/{flightId}")]
        public async Task<ActionResult<IEnumerable<BookingReadDto>>> GetBookingsByFlight(int flightId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.flight_id == flightId)
                .ToListAsync();

            if (bookings == null || !bookings.Any())
            {
                return NotFound();
            }

            var bookingDtos = bookings.Select(b => new BookingReadDto
            {
                BookingId = b.booking_id,
                FlightId = b.flight_id,
                PassengerId = b.passenger_id,
                Seat = b.seat,
                Price = b.price
            }).ToList();

            return Ok(bookingDtos);
        }

        // GET: api/booking/passenger/{passengerId}
        [HttpGet("passenger/{passengerId}")]
        public async Task<ActionResult<IEnumerable<BookingReadDto>>> GetBookingsByPassenger(int passengerId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.passenger_id == passengerId)
                .ToListAsync();

            if (bookings == null || !bookings.Any())
            {
                return NotFound();
            }

            var bookingDtos = bookings.Select(b => new BookingReadDto
            {
                BookingId = b.booking_id,
                FlightId = b.flight_id,
                PassengerId = b.passenger_id,
                Seat = b.seat,
                Price = b.price
            }).ToList();

            return Ok(bookingDtos);
        }

        // POST: api/booking
        [HttpPost]
        public async Task<ActionResult<BookingReadDto>> CreateBooking(BookingCreateDto bookingCreateDto)
        {
            var booking = new Booking
            {
                flight_id = bookingCreateDto.FlightId,
                passenger_id = bookingCreateDto.PassengerId,
                seat = bookingCreateDto.Seat,
                price = bookingCreateDto.Price
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var bookingDto = new BookingReadDto
            {
                BookingId = booking.booking_id,
                FlightId = booking.flight_id,
                PassengerId = booking.passenger_id,
                Seat = booking.seat,
                Price = booking.price
            };

            return CreatedAtAction(nameof(GetBooking), new { id = booking.booking_id }, bookingDto);
        }

        // PUT: api/booking/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, BookingUpdateDto bookingUpdateDto)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            booking.flight_id = bookingUpdateDto.FlightId;
            booking.passenger_id = bookingUpdateDto.PassengerId;
            booking.seat = bookingUpdateDto.Seat;
            booking.price = bookingUpdateDto.Price;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BookingExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/booking/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> BookingExists(int id)
        {
            return await _context.Bookings.AnyAsync(e => e.booking_id == id);
        }
    }
}
