using Hotel_Managment.Data;
using Hotel_Managment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Managment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public DiscountController(HotelDbContext context)
        {
            _context = context;
        }

        // ✅ Get All Discounts (Admin only)
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDiscounts()
        {
            var discounts = await _context.Discounts.ToListAsync();
            return Ok(discounts);
        }

        // ✅ Get Discount by Id (Admin only)
        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDiscount(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
                return NotFound();

            return Ok(discount);
        }

        // ✅ Apply Discount by Code (Customer)
        [HttpGet("apply/{code}")]
        //[Authorize(Roles = "Customer")]
        public async Task<IActionResult> ApplyDiscount(string code)
        {
            var discount = await _context.Discounts.FirstOrDefaultAsync(d => d.Code == code && d.IsActive);
            if (discount == null)
                return NotFound("Invalid or inactive discount code.");

            if (discount.ExpiryDate < DateTime.UtcNow)
                return BadRequest("Discount expired.");

            if (discount.UsageLimit <= 0)
                return BadRequest("Discount usage limit reached.");

            return Ok(discount);
        }

        // ✅ Create Discount (Admin only)
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDiscount([FromBody] Discount discount)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //foreach (var d in discount)
            //{
            //    d.ExpiryDate = DateTime.SpecifyKind(d.ExpiryDate, DateTimeKind.Utc);
            //}
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(GetDiscount), new { id = discount.DistId }, discount);
           
        }

        // ✅ Update Discount (Admin only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDiscount(int id, [FromBody] Discount discount)
        {
            if (id != discount.DistId)
                return BadRequest();

            _context.Entry(discount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Discounts.Any(d => d.DistId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // ✅ Delete Discount (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
                return NotFound();

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
