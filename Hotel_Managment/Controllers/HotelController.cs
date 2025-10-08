using Hotel_Managment.Data;
using Hotel_Managment.DTOs;
using Hotel_Managment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Managment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public HotelController(HotelDbContext context)
        {
            _context = context;
        }

        // Get All Hotels
        // Get All Hotels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetHotels()
        {
            var hotels = await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.Reviews)
                .Include(h => h.Owner)
                .Include(h => h.HotelImages)
                .Select(h => new
                {
                    h.HotelId,
                    h.HotelName,
                    h.Description,
                    h.PhoneNumber,
                    h.Address,
                    h.StarRating,
                    MainImageUrl = h.HotelImages
                        .Where(img => img.IsMain)
                        .Select(img => $"{Request.Scheme}://{Request.Host}{img.ImageUrl}")
                        .FirstOrDefault(), // ✅ الصورة الرئيسية فقط
                    Owner = h.Owner == null ? null : new
                    {
                        h.Owner.Id,
                        h.Owner.UserName,
                        h.Owner.Email,
                        h.Owner.Country,
                        h.Owner.City
                    },
                    Rooms = h.Rooms.Select(r => new
                    {
                        r.RoomId,
                        r.RoomNumber,
                        r.PricePerNight,
                        r.IsAvailable
                    }),
                    Reviews = h.Reviews.Select(rv => new
                    {
                        rv.ReviewId,
                        rv.Rating,
                        rv.Comment,
                        rv.CreatedAt,
                        User = new { rv.User.Id, rv.User.UserName, rv.User.Email }
                    }),
                    HotelImages = h.HotelImages.Select(hi => new
                    {
                        hi.ImageId,
                        ImageUrl = $"{Request.Scheme}://{Request.Host}{hi.ImageUrl}",
                        hi.IsMain
                    })
                })
                .ToListAsync();

            return hotels;
        }


        // Get Hotel by Id
        // Get Hotel by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetHotel(int id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.Reviews)
                .Include(h => h.Owner)
                .Include(h => h.HotelImages)
                .Where(h => h.HotelId == id)
                .Select(h => new
                {
                    h.HotelId,
                    h.HotelName,
                    h.Description,
                    h.PhoneNumber,
                    h.Address,
                    h.StarRating,
                    MainImageUrl = h.HotelImages
                        .Where(img => img.IsMain)
                        .Select(img => $"{Request.Scheme}://{Request.Host}{img.ImageUrl}")
                        .FirstOrDefault(), // ✅ صورة رئيسية
                    Owner = h.Owner == null ? null : new
                    {
                        h.Owner.Id,
                        h.Owner.UserName,
                        h.Owner.Email,
                        h.Owner.Country,
                        h.Owner.City
                    },
                    Rooms = h.Rooms.Select(r => new
                    {
                        r.RoomId,
                        r.RoomNumber,
                        r.PricePerNight,
                        r.IsAvailable
                    }),
                    Reviews = h.Reviews.Select(rv => new
                    {
                        rv.ReviewId,
                        rv.Rating,
                        rv.Comment,
                        rv.CreatedAt,
                        User = new { rv.User.Id, rv.User.UserName, rv.User.Email }
                    }),
                    HotelImages = h.HotelImages.Select(hi => new
                    {
                        hi.ImageId,
                        ImageUrl = $"{Request.Scheme}://{Request.Host}{hi.ImageUrl}",
                        hi.IsMain
                    })
                })
                .FirstOrDefaultAsync();

            if (hotel == null)
            {
                return NotFound();
            }

            return hotel;
        }


        // Create Hotel
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<Hotel>> CreateHotel([FromBody] Hotel hotel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHotel), new { id = hotel.HotelId }, hotel);
        }

        // Update Hotel
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] Hotel hotel)
        {
            if (id != hotel.HotelId)
            {
                return BadRequest("Hotel ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(hotel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Hotels.Any(e => e.HotelId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Delete Hotel
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
