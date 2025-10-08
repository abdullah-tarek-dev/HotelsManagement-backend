using Hotel_Managment.Data;
using Hotel_Managment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Managment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public RoomController(HotelDbContext context)
        {
            _context = context;
        }

        // ✅ Get All Rooms
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .Include(r => r.RoomAmenities)
                    .ThenInclude(ra => ra.Amenity)
                .Select(r => new
                {
                    r.RoomId,
                    r.RoomNumber,
                    r.PricePerNight,
                    r.IsAvailable,
                    Hotel = new { r.Hotel.HotelId, r.Hotel.HotelName },
                    RoomType = new { r.RoomType.RoomTypeId, r.RoomType.RoomName },
                    Amenities = r.RoomAmenities.Select(ra => new
                    {
                        ra.Amenity.AmenityId,
                        ra.Amenity.Name,
                        ra.Amenity.Description
                    })
                })
                .ToListAsync();

            return Ok(rooms);
        }

        // ✅ Get Room By Id
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoom(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .Include(r => r.RoomAmenities)
                    .ThenInclude(ra => ra.Amenity)
                .Where(r => r.RoomId == id)
                .Select(r => new
                {
                    r.RoomId,
                    r.RoomNumber,
                    r.PricePerNight,
                    r.IsAvailable,
                    Hotel = new { r.Hotel.HotelId, r.Hotel.HotelName },
                    RoomType = new { r.RoomType.RoomTypeId, r.RoomType.RoomName },
                    Amenities = r.RoomAmenities.Select(ra => new
                    {
                        ra.Amenity.AmenityId,
                        ra.Amenity.Name,
                        ra.Amenity.Description
                    })
                })
                .FirstOrDefaultAsync();

            if (room == null)
                return NotFound();

            return Ok(room);
        }

        // ✅ Create Room (Admin Only)
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRoom([FromBody] Room room)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _context.Hotels.AnyAsync(h => h.HotelId == room.HotelId) ||
                !await _context.RoomTypes.AnyAsync(rt => rt.RoomTypeId == room.RoomTypeId))
            {
                return BadRequest("HotelId or RoomTypeId is invalid.");
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoom), new { id = room.RoomId }, room);
        }

        // ✅ Update Room (Admin Only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] Room room)
        {
            if (id != room.RoomId)
                return BadRequest();

            var existingRoom = await _context.Rooms.FindAsync(id);
            if (existingRoom == null)
                return NotFound();

            if (!await _context.Hotels.AnyAsync(h => h.HotelId == room.HotelId) ||
                !await _context.RoomTypes.AnyAsync(rt => rt.RoomTypeId == room.RoomTypeId))
            {
                return BadRequest("HotelId or RoomTypeId is invalid.");
            }

            existingRoom.RoomNumber = room.RoomNumber;
            existingRoom.PricePerNight = room.PricePerNight;
            existingRoom.IsAvailable = room.IsAvailable;
            existingRoom.HotelId = room.HotelId;
            existingRoom.RoomTypeId = room.RoomTypeId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ Delete Room (Admin Only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //// ✅ Assign Amenity to Room
        //[HttpPost("{roomId}/amenities/{amenityId}")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> AssignAmenityToRoom(int roomId, int amenityId)
        //{
        //    var room = await _context.Rooms.FindAsync(roomId);
        //    var amenity = await _context.Amenities.FindAsync(amenityId);

        //    if (room == null || amenity == null)
        //        return BadRequest("Invalid RoomId or AmenityId");

        //    var exists = await _context.RoomAmenities
        //        .AnyAsync(ra => ra.RoomId == roomId && ra.AmenityId == amenityId);

        //    if (exists)
        //        return BadRequest("Amenity already assigned to this room.");

        //    var roomAmenity = new RoomAmenity
        //    {
        //        RoomId = roomId,
        //        AmenityId = amenityId
        //    };

        //    _context.RoomAmenities.Add(roomAmenity);
        //    await _context.SaveChangesAsync();

        //    return Ok("Amenity assigned successfully.");
        //}

        //// ✅ Remove Amenity from Room
        //[HttpDelete("{roomId}/amenities/{amenityId}")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> RemoveAmenityFromRoom(int roomId, int amenityId)
        //{
        //    var roomAmenity = await _context.RoomAmenities
        //        .FirstOrDefaultAsync(ra => ra.RoomId == roomId && ra.AmenityId == amenityId);

        //    if (roomAmenity == null)
        //        return NotFound("Amenity not assigned to this room.");

        //    _context.RoomAmenities.Remove(roomAmenity);
        //    await _context.SaveChangesAsync();

        //    return Ok("Amenity removed successfully.");
        //}
    }
}
