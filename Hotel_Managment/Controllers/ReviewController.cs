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
    public class ReviewController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public ReviewController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: api/Review
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews()
        {
            return await _context.Reviews
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UserId = r.UserId,
                    HotelId = r.HotelId
                })
                .ToListAsync();
        }

        // GET: api/Review/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            var review = await _context.Reviews
                .Where(r => r.ReviewId == id)
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UserId = r.UserId,
                    HotelId = r.HotelId
                })
                .FirstOrDefaultAsync();

            if (review == null)
                return NotFound();

            return Ok(review);
        }

        // GET: api/Review/ByHotel/3
        [HttpGet("ByHotel/{hotelId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByHotel(int hotelId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.HotelId == hotelId)
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UserId = r.UserId,
                    HotelId = r.HotelId
                })
                .ToListAsync();

            return Ok(reviews);
        }

        // POST: api/Review
        [HttpPost]
        //[Authorize] 
        public async Task<ActionResult<ReviewDto>> PostReview(Review review)
        {
            if (review.Rating < 1 || review.Rating > 5)
                return BadRequest("Rating must be between 1 and 5.");

            review.CreatedAt = DateTime.UtcNow;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var dto = new ReviewDto
            {
                ReviewId = review.ReviewId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                UserId = review.UserId,
                HotelId = review.HotelId
            };

            return CreatedAtAction(nameof(GetReview), new { id = review.ReviewId }, dto);
        }

        // PUT: api/Review/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutReview(int id, ReviewDto reviewDto)
        {
            if (id != reviewDto.ReviewId)
                return BadRequest();

            var existingReview = await _context.Reviews.FindAsync(id);
            if (existingReview == null)
                return NotFound();

            existingReview.Rating = reviewDto.Rating;
            existingReview.Comment = reviewDto.Comment;
            existingReview.HotelId = reviewDto.HotelId;
            existingReview.UserId = reviewDto.UserId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Review/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Review/AverageRating/3
        [HttpGet("AverageRating/{hotelId}")]
        public async Task<ActionResult<object>> GetAverageRating(int hotelId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.HotelId == hotelId)
                .ToListAsync();

            if (!reviews.Any())
                return Ok(new { HotelId = hotelId, AverageRating = 0, TotalReviews = 0 });

            var avg = reviews.Average(r => r.Rating);

            return Ok(new
            {
                HotelId = hotelId,
                AverageRating = Math.Round(avg, 2),
                TotalReviews = reviews.Count
            });
        }

        // GET: api/Review/TopRated/5
        [HttpGet("TopRated/{count}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTopRatedHotels(int count)
        {
            var topHotels = await _context.Reviews
                .GroupBy(r => r.HotelId)
                .Select(g => new
                {
                    HotelId = g.Key,
                    AverageRating = g.Average(r => r.Rating),
                    TotalReviews = g.Count()
                })
                .OrderByDescending(h => h.AverageRating)
                .ThenByDescending(h => h.TotalReviews)
                .Take(count)
                .ToListAsync();

            return Ok(topHotels);
        }
    }
}
