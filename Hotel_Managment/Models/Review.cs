using System.ComponentModel.DataAnnotations;

namespace Hotel_Managment.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }
    }
}
