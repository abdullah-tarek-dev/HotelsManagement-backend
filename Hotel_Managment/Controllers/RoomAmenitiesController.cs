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
    public class RoomAmenitiesController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public RoomAmenitiesController(HotelDbContext context)
        {
            _context = context;
        }

        // ✅ Get All RoomAmenities
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RoomAmenityDto>>> GetRoomAmenities()
        {
            var result = await _context.RoomAmenities
                .Include(ra => ra.Room)
                    .ThenInclude(r => r.Hotel)
                .Include(ra => ra.Amenity)
                .Select(ra => new RoomAmenityDto
                {
                    RoomId = ra.RoomId,
                    //RoomNumber = ra.Room.RoomNumber,
                    HotelId = ra.Room.Hotel.HotelId,
                    //HotelName = ra.Room.Hotel.HotelName,
                    AmenityId = ra.AmenityId,
                    //AmenityName = ra.Amenity.Name,
                    //AmenityDescription = ra.Amenity.Description
                })
                .ToListAsync();

            return Ok(result);
        }

        // ✅ Get Amenities by RoomId
        [HttpGet("room/{roomId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RoomAmenityDto>>> GetAmenitiesByRoom(int roomId)
        {
            var roomAmenities = await _context.RoomAmenities
                .Include(ra => ra.Room)
                    .ThenInclude(r => r.Hotel)
                .Include(ra => ra.Amenity)
                .Where(ra => ra.RoomId == roomId)
                .Select(ra => new RoomAmenityDto
                {
                    RoomId = ra.RoomId,
                    //RoomNumber = ra.Room.RoomNumber,
                    HotelId = ra.Room.Hotel.HotelId,
                    //HotelName = ra.Room.Hotel.HotelName,
                    AmenityId = ra.AmenityId,
                    //AmenityName = ra.Amenity.Name,
                    //AmenityDescription = ra.Amenity.Description
                })
                .ToListAsync();

            if (!roomAmenities.Any())
                return NotFound("No amenities found for this room.");

            return Ok(roomAmenities);
        }

        // ✅ Add RoomAmenity (Admin Only)
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [HttpPost("AddRoomAmenities")]
        public async Task<IActionResult> AddRoomAmenity([FromBody] List<RoomAmenity> roomAmenities)
        {
            if (roomAmenities == null || roomAmenities.Count == 0)
                return BadRequest("Room amenities list cannot be empty.");

            foreach (var ra in roomAmenities)
            {
                // check valid RoomId
                if (!await _context.Rooms.AnyAsync(r => r.RoomId == ra.RoomId))
                    return BadRequest($"Invalid RoomId: {ra.RoomId}");

                // check valid AmenityId
                if (!await _context.Amenities.AnyAsync(a => a.AmenityId == ra.AmenityId))
                    return BadRequest($"Invalid AmenityId: {ra.AmenityId}");

                // check duplicate
                var exists = await _context.RoomAmenities
                    .AnyAsync(x => x.RoomId == ra.RoomId && x.AmenityId == ra.AmenityId);

                if (exists)
                    return BadRequest($"AmenityId {ra.AmenityId} is already assigned to RoomId {ra.RoomId}");
            }

            _context.RoomAmenities.AddRange(roomAmenities);
            await _context.SaveChangesAsync();

            return Ok("Room amenities added successfully.");
        }


        // ✅ Delete RoomAmenity (Admin Only)
        [HttpDelete("{roomId}/{amenityId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoomAmenity(int roomId, int amenityId)
        {
            var ra = await _context.RoomAmenities
                .FirstOrDefaultAsync(x => x.RoomId == roomId && x.AmenityId == amenityId);

            if (ra == null) return NotFound("Amenity not assigned to this room.");

            _context.RoomAmenities.Remove(ra);
            await _context.SaveChangesAsync();

            return Ok("Room amenity deleted successfully.");
        }
    }
}
