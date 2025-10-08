using Hotel_Managment.Data;
using Hotel_Managment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Managment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingStatusController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public BookingStatusController(HotelDbContext context)
        {
            _context = context;
        }

        // ✅ Get All Statuses
        [HttpGet]
        [AllowAnonymous] // الكل ممكن يشوفها
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await _context.BookingStatuses.ToListAsync();
            return Ok(statuses);
        }

        // ✅ Get Status By Id
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStatus(int id)
        {
            var status = await _context.BookingStatuses.FindAsync(id);
            if (status == null)
                return NotFound();

            return Ok(status);
        }

        // ✅ Create Status (Admin only)
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStatus([FromBody] BookingStatus status)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.BookingStatuses.Add(status);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStatus), new { id = status.StatusId }, status);
        }

        // ✅ Update Status (Admin only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] BookingStatus status)
        {
            if (id != status.StatusId)
                return BadRequest();

            _context.Entry(status).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.BookingStatuses.Any(s => s.StatusId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // ✅ Delete Status (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            var status = await _context.BookingStatuses.FindAsync(id);
            if (status == null)
                return NotFound();

            _context.BookingStatuses.Remove(status);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
