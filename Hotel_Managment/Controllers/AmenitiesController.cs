using Hotel_Managment.Data;
using Hotel_Managment.Models;
using Hotel_Managment.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Managment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmenitiesController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public AmenitiesController(HotelDbContext context)
        {
            _context = context;
        }

        // ✅ Get All Amenities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AmenityDto>>> GetAmenities()
        {
            var amenities = await _context.Amenities
                .Include(a => a.RoomAmenities)
                .ThenInclude(ra => ra.Room)
                .Select(a => new AmenityDto
                {
                    AmenityId = a.AmenityId,
                    Name = a.Name,
                    Description = a.Description
                })
                .ToListAsync();

            return Ok(amenities);
        }

        // ✅ Get Amenity by Id
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetAmenity(int id)
        {
            var amenity = await _context.Amenities
                .Include(a => a.RoomAmenities)
                .ThenInclude(ra => ra.Room)
                .Where(a => a.AmenityId == id)
                 .Select(a => new AmenityDto
          {
              AmenityId = a.AmenityId,
              Name = a.Name,
              Description = a.Description
          })
                .FirstOrDefaultAsync();

            if (amenity == null)
                return NotFound();

            return Ok(amenity);
        }

        // ✅ Create Amenity
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AmenityDto>> CreateAmenity([FromBody] List<Amenity> amenities)
        {
            if (amenities == null || amenities.Count == 0)
                return BadRequest("Amenities list cannot be empty");

            _context.Amenities.AddRange(amenities);
            await _context.SaveChangesAsync();

            var amenitiesDto = amenities.Select(a => new AmenityDto
            {
                AmenityId = a.AmenityId,
                Name = a.Name,
                Description = a.Description
            }).ToList();

            return CreatedAtAction(nameof(GetAmenity), amenitiesDto);
        }

        // ✅ Update Amenity
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAmenity(int id, Amenity amenity)
        {
            if (id != amenity.AmenityId)
                return BadRequest("Amenity ID mismatch");

            _context.Entry(amenity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Amenities.Any(e => e.AmenityId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // ✅ Delete Amenity
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAmenity(int id)
        {
            var amenity = await _context.Amenities.FindAsync(id);
            if (amenity == null)
                return NotFound();

            _context.Amenities.Remove(amenity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
