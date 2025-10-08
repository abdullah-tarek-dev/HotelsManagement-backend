using System.ComponentModel.DataAnnotations;

namespace Hotel_Managment.Models
{
    public class BookingStatus
    {
        [Key]
        public int StatusId { get; set; }
        [Required]
        [MaxLength(50)]
        public string StatusName { get; set; }

        public string? Description { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
