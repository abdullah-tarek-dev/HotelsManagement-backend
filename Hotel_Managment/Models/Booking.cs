using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Managment.Models
{
    public class Booking
    {
        [Key]
       
        public int BookingId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumOfGuests { get; set; }
        public decimal TotalAmount { get; set; }

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int RoomId { get; set; }
        public Room? Room { get; set; }

        public int StatusId { get; set; }
        public BookingStatus? Status { get; set; }

        public int? DiscountId { get; set; }
        public Discount? Discount { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
