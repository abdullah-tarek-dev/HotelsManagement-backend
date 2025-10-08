using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Managment.DTOs
{
    public class BookingDto
    {
       
        public int BookingId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }

        public int RoomId { get; set; }
        public string RoomName { get; set; }   // 👈 جديد

        public string UserId { get; set; }
        public string UserName { get; set; }   // 👈 جديد

        public int StatusId { get; set; }
        public string StatusName { get; set; } // 👈 جديد

        public int DiscountId { get; set; }
        public string DiscountCode { get; set; }
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
