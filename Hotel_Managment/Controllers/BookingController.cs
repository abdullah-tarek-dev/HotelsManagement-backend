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
    public class BookingController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public BookingController(HotelDbContext context)
        {
            _context = context;
        }

        // ✅ Get All Bookings (Admin Only)
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .Include(b => b.Status)
                .Include(b => b.Discount)
                .Include(b => b.Payments)
                .ToListAsync();
            var result = bookings.Select(b => new BookingDto
            {
                BookingId = b.BookingId,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                TotalAmount = b.TotalAmount,

                RoomId = b.RoomId,
                RoomName = b.Room?.RoomNumber,   // ✅ اسم أو رقم الغرفة

                UserId = b.UserId,
                UserName = b.User?.UserName,     // ✅ اسم اليوزر

                StatusId = b.StatusId,
                StatusName = b.Status?.StatusName,     // ✅ اسم الحالة

                DiscountId = b.DiscountId ?? 0,
                DiscountCode = b.Discount?.Code,
                DiscountValue = b.Discount?.DisValue ?? 0,
                IsPercentage = b.Discount?.IsPercentage ?? false,


                CreatedAt = b.CreatedAt
            });


            return Ok(result);
        }

        // ✅ Get Booking By Id (Admin or Owner)
        [HttpGet("{id}")]
        //[Authorize]
        public async Task<IActionResult> GetBooking(int id)
        {
            var b = await _context.Bookings
                .Include(x => x.User)
                .Include(x => x.Room)
                .Include(x => x.Status)
                .Include(x => x.Discount)
                .Include(x => x.Payments)
                .FirstOrDefaultAsync(x => x.BookingId == id);

            if (b == null)
                return NotFound();

            // لو يوزر عادي → يقدر يشوف بس حجوزاته
            // مؤقت للتجربه
            //if (!User.IsInRole("Admin") && b.UserId != User.FindFirst("sub")?.Value)
            //    return Forbid();

            var dto = new BookingDto
            {
                BookingId = b.BookingId,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                TotalAmount = b.TotalAmount,

                RoomId = b.RoomId,
                RoomName = b.Room?.RoomNumber,

                UserId = b.UserId,
                UserName = b.User?.UserName,

                StatusId = b.StatusId,
                StatusName = b.Status?.StatusName,

                DiscountId = b.DiscountId ?? 0,
                DiscountCode = b.Discount?.Code,
                DiscountValue = b.Discount?.DisValue ?? 0,
                IsPercentage = b.Discount?.IsPercentage ?? false,


                CreatedAt = b.CreatedAt
            };


            return Ok(dto);
        }

        // ✅ Create Bookings (Customer)
        [HttpPost]
        //[Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateBooking([FromBody] List<BookingDto> bookingDtos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst("sub")?.Value;

            var bookings = bookingDtos.Select(dto => new Booking
            {
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                TotalAmount = dto.TotalAmount,
                RoomId = dto.RoomId,
                UserId = userId ?? dto.UserId,
                StatusId = dto.StatusId,
                DiscountId = dto.DiscountId,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _context.Bookings.AddRange(bookings);
            await _context.SaveChangesAsync();

            var result = bookings.Select(b => new BookingDto
            {
                BookingId = b.BookingId,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                TotalAmount = b.TotalAmount,
                RoomId = b.RoomId,
                UserId = b.UserId,
                StatusId = b.StatusId,
                DiscountId = b.DiscountId ?? 0
            }).ToList();

            return Ok(result);
        }

        // ✅ Update Booking (Admin Only)
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingDto dto)
        {
            if (id != dto.BookingId)
                return BadRequest();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            booking.CheckInDate = dto.CheckInDate;
            booking.CheckOutDate = dto.CheckOutDate;
            booking.TotalAmount = dto.TotalAmount;
            booking.RoomId = dto.RoomId;
            booking.StatusId = dto.StatusId;
            booking.DiscountId = dto.DiscountId;

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Bookings.Any(b => b.BookingId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // ✅ Delete Booking (Admin Only)
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

/*
 [
  {
    "bookingId": 1,
    "checkInDate": "2025-10-05T14:00:00Z",
    "checkOutDate": "2025-10-10T12:00:00Z",
    "totalAmount": 750,
    "roomId": 1,
    "roomName": "Deluxe Room",
    "userId": "dd5cf8d7-ada6-43d9-b1be-8665420db426",
    "userName": "Ahmed Ali",
    "statusId": 1,
    "statusName": "Confirmed",
    "discountId": 1,
    "discountCode": "DISC10",
    "discountValue": 10,
    "isPercentage": true,
    "createdAt": "2025-10-02T09:00:00Z"
  },
  {
    "bookingId": 2,
    "checkInDate": "2025-10-12T15:00:00Z",
    "checkOutDate": "2025-10-15T12:00:00Z",
    "totalAmount": 450,
    "roomId": 2,
    "roomName": "Standard Room",
    "userId": "a851a614-6ed8-4e88-b1c3-7651ddf38654",
    "userName": "Sara Mohamed",
    "statusId": 2,
    "statusName": "Pending",
    "discountId": 2,
    "discountCode": "",
    "discountValue": 0,
    "isPercentage": false,
    "createdAt": "2025-10-02T09:10:00Z"
  },
  {
    "bookingId": 3,
    "checkInDate": "2025-10-20T14:00:00Z",
    "checkOutDate": "2025-10-25T12:00:00Z",
    "totalAmount": 1200,
    "roomId": 3,
    "roomName": "Suite",
    "userId": "29c083c3-810e-4a93-aee9-fb30f07636d3",
    "userName": "Mahmoud Hassan",
    "statusId": 1,
    "statusName": "Confirmed",
    "discountId": 3,
    "discountCode": "VIP20",
    "discountValue": 20,
    "isPercentage": true,
    "createdAt": "2025-10-02T09:15:00Z"
  },
  {
    "bookingId": 4,
    "checkInDate": "2025-10-08T14:00:00Z",
    "checkOutDate": "2025-10-09T12:00:00Z",
    "totalAmount": 150,
    "roomId": 4,
    "roomName": "Single Room",
    "userId": "bccb65c6-ca93-49bc-b08d-7e9112d4f219",
    "userName": "Omar Farouk",
    "statusId": 3,
    "statusName": "Cancelled",
    "discountId": 4,
    "discountCode": "",
    "discountValue": 0,
    "isPercentage": false,
    "createdAt": "2025-10-02T09:20:00Z"
  },
  {
    "bookingId": 5,
    "checkInDate": "2025-11-01T14:00:00Z",
    "checkOutDate": "2025-11-07T12:00:00Z",
    "totalAmount": 1800,
    "roomId": 5,
    "roomName": "Presidential Suite",
    "userId": "eb08c7be-c0b7-42dd-8cd4-e13e507b1997",
    "userName": "Laila Said",
    "statusId": 1,
    "statusName": "Confirmed",
    "discountId": 5,
    "discountCode": "PRES15",
    "discountValue": 15,
    "isPercentage": true,
    "createdAt": "2025-10-02T09:30:00Z"
  },
  {
    "bookingId": 6,
    "checkInDate": "2025-10-18T14:00:00Z",
    "checkOutDate": "2025-10-22T12:00:00Z",
    "totalAmount": 600,
    "roomId": 6,
    "roomName": "Twin Room",
    "userId": "39135a8b-1942-4143-a2c8-597689fb191f",
    "userName": "Youssef Adel",
    "statusId": 2,
    "statusName": "Pending",
    "discountId": 6,
    "discountCode": "",
    "discountValue": 0,
    "isPercentage": false,
    "createdAt": "2025-10-02T09:35:00Z"
  },
  {
    "bookingId": 7,
    "checkInDate": "2025-12-24T14:00:00Z",
    "checkOutDate": "2025-12-31T12:00:00Z",
    "totalAmount": 3000,
    "roomId": 7,
    "roomName": "Family Suite",
    "userId": "6ffe6049-e00e-4c1c-bd36-4335dfecbfd4",
    "userName": "Hana Ibrahim",
    "statusId": 1,
    "statusName": "Confirmed",
    "discountId": 7,
    "discountCode": "NEWYEAR25",
    "discountValue": 25,
    "isPercentage": true,
    "createdAt": "2025-10-02T09:45:00Z"
  },
  {
    "bookingId": 8,
    "checkInDate": "2025-09-28T14:00:00Z",
    "checkOutDate": "2025-10-02T12:00:00Z",
    "totalAmount": 900,
    "roomId": 8,
    "roomName": "Double Room",
    "userId": "8e0941bb-d8a3-409e-8a87-36fee6b4c87c",
    "userName": "Mostafa Gamal",
    "statusId": 1,
    "statusName": "Confirmed",
    "discountId": 8,
    "discountCode": "",
    "discountValue": 0,
    "isPercentage": false,
    "createdAt": "2025-09-27T20:00:00Z"
  },
  {
    "bookingId": 9,
    "checkInDate": "2025-10-03T14:00:00Z",
    "checkOutDate": "2025-10-06T12:00:00Z",
    "totalAmount": 500,
    "roomId": 9,
    "roomName": "Economy Room",
    "userId": "14344fcf-f2b1-4d00-889f-cf0d98396212",
    "userName": "Karim Nasser",
    "statusId": 2,
    "statusName": "Pending",
    "discountId": 9,
    "discountCode": "",
    "discountValue": 0,
    "isPercentage": false,
    "createdAt": "2025-10-02T10:00:00Z"
  },
  {
    "bookingId": 10,
    "checkInDate": "2025-11-15T14:00:00Z",
    "checkOutDate": "2025-11-20T12:00:00Z",
    "totalAmount": 1100,
    "roomId": 10,
    "roomName": "Deluxe Room",
    "userId": "750c01b8-d34e-420f-8508-2a60c86d7b2f",
    "userName": "Nourhan Tarek",
    "statusId": 1,
    "statusName": "Confirmed",
    "discountId": 10,
    "discountCode": "DELUXE5",
    "discountValue": 5,
    "isPercentage": true,
    "createdAt": "2025-10-02T10:10:00Z"
  }
]

 */