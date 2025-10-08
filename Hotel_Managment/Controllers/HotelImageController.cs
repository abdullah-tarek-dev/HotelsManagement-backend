using Hotel_Managment.Data;
using Hotel_Managment.DTOs;
using Hotel_Managment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Managment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelImageController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public HotelImageController(HotelDbContext context)
        {
            _context = context;
        }

        // ✅ Get Base URL once
        private string GetBaseUrl()
        {
            return $"{Request.Scheme}://{Request.Host}";
        }

        // GET: api/HotelImage
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelImageDto>>> GetHotelImages()
        {
            var baseUrl = GetBaseUrl();

            var images = await _context.HotelImages
                .Select(i => new HotelImageDto
                {
                    ImageId = i.ImageId,
                    ImageUrl = baseUrl + i.ImageUrl,
                    //Caption = i.Caption,
                    IsMain = i.IsMain,
                    HotelId = i.HotelId,
                }).ToListAsync();

            return Ok(images);
        }

        // GET: api/HotelImage/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelImageDto>> GetHotelImage(int id)
        {
            var baseUrl = GetBaseUrl();

            var image = await _context.HotelImages
                .Where(i => i.ImageId == id)
                .Select(i => new HotelImageDto
                {
                    ImageId = i.ImageId,
                    ImageUrl = baseUrl + i.ImageUrl,
                    //Caption = i.Caption,
                    IsMain = i.IsMain,
                    HotelId = i.HotelId,
                }).FirstOrDefaultAsync();

            if (image == null)
            {
                return NotFound();
            }

            return Ok(image);
        }

        // ✅ GET: api/HotelImage/Hotel/3 => صور فندق محدد
        [HttpGet("Hotel/{hotelId}")]
        public async Task<ActionResult<IEnumerable<HotelImageDto>>> GetHotelImagesByHotel(int hotelId)
        {
            var baseUrl = GetBaseUrl();

            var images = await _context.HotelImages
                .Where(i => i.HotelId == hotelId)
                .Select(i => new HotelImageDto
                {
                    ImageId = i.ImageId,
                    ImageUrl = baseUrl + i.ImageUrl,
                    IsMain = i.IsMain
                }).ToListAsync();

            if (!images.Any())
            {
                return NotFound("No images found for this hotel.");
            }

            return Ok(images);
        }

        // POST: api/HotelImage (لو عاوز تبعتها Manual في Body)
        [HttpPost]
        
        public async Task<ActionResult<IEnumerable<HotelImageDto>>> PostHotelImage([FromBody] List<HotelImage> hotelImages)
        {
            if (hotelImages == null || hotelImages.Count == 0)
            {
                return BadRequest("No hotel images provided");
            }

            _context.HotelImages.AddRange(hotelImages);
            await _context.SaveChangesAsync();

            var baseUrl = GetBaseUrl();

            // تحويل كل كائن إلى DTO
            var dtoList = hotelImages.Select(img => new HotelImageDto
            {
                ImageId = img.ImageId,
                ImageUrl = baseUrl + img.ImageUrl,
                //Caption = img.Caption,
                IsMain = img.IsMain,
                HotelId = img.HotelId
            }).ToList();

            return Created("api/HotelImage", dtoList);
        }


        // ✅ Upload Endpoint: api/HotelImage/upload
        [HttpPost("upload")]
        [Consumes("multipart/form-data")] // مهم جداً عشان Swagger
        public async Task<IActionResult> UploadImage([FromForm] HotelImageUploadDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("No file uploaded.");

            var folderPath = Path.Combine("wwwroot", "uploads", "hotels");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.File.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            var relativePath = "/uploads/hotels/" + fileName;

            var hotelImage = new HotelImage
            {
                HotelId = dto.HotelId,
                ImageUrl = relativePath,
                Caption = dto.Caption,
                IsMain = dto.IsMain
            };

            _context.HotelImages.Add(hotelImage);
            await _context.SaveChangesAsync();

            var baseUrl = GetBaseUrl();
            var response = new HotelImageDto
            {
                ImageId = hotelImage.ImageId,
                ImageUrl = baseUrl + relativePath,
                //Caption = hotelImage.Caption,
                IsMain = hotelImage.IsMain
            };

            return Ok(response);
        }


        // PUT: api/HotelImage/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotelImage(int id, HotelImage hotelImage)
        {
            if (id != hotelImage.ImageId)
            {
                return BadRequest();
            }

            _context.Entry(hotelImage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelImageExists(id))
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

        // DELETE: api/HotelImage/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotelImage(int id)
        {
            var hotelImage = await _context.HotelImages.FindAsync(id);
            if (hotelImage == null)
            {
                return NotFound();
            }

            // كمان نحذف الفايل من السيرفر
            var filePath = Path.Combine("wwwroot", hotelImage.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.HotelImages.Remove(hotelImage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HotelImageExists(int id)
        {
            return _context.HotelImages.Any(e => e.ImageId == id);
        }
    }
}
