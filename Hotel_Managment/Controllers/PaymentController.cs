using Hotel_Managment.Data;
using Hotel_Managment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Managment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public PaymentController(HotelDbContext context)
        {
            _context = context;
        }

        // ✅ Get All Payments (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPayments()
        {
            var payments = await _context.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.User)
                .ToListAsync();

            return Ok(payments);
        }

        // ✅ Get Payment By Id (Admin or Owner of booking)
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetPayment(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
                return NotFound();

            // لو يوزر عادي → يقدر يشوف بس مدفوعاته
            if (!User.IsInRole("Admin") && payment.Booking.UserId != User.FindFirst("sub")?.Value)
                return Forbid();

            return Ok(payment);
        }

        // ✅ Create Payment (Customer only)
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreatePayment([FromBody] Payment payment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var booking = await _context.Bookings.FindAsync(payment.BookingId);
            if (booking == null)
                return BadRequest("Booking not found");

            // تأكد ان اليوزر صاحب الحجز
            if (booking.UserId != User.FindFirst("sub")?.Value)
                return Forbid();

            payment.PayDate = DateTime.UtcNow;

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPayment), new { id = payment.PaymentId }, payment);
        }

        // ✅ Update Payment (Admin only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] Payment payment)
        {
            if (id != payment.PaymentId)
                return BadRequest();

            _context.Entry(payment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Payments.Any(p => p.PaymentId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // ✅ Delete Payment (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound();

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
