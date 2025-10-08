using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Managment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly HotelReportService _reportService;

        public ReportsController(HotelReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("top-hotels")]
        public async Task<IActionResult> GetTopHotels([FromQuery] int limit = 10)
        {
            var hotels = await _reportService.GetTopHotelsAsync(limit);
            return Ok(hotels);
        }
    }
}
