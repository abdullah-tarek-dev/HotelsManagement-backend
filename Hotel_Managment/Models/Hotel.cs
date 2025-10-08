using System.ComponentModel.DataAnnotations;

namespace Hotel_Managment.Models
{
    public class Hotel
    {
        [Key]
        public int HotelId { get; set; }
        [Required]
        [MaxLength(200)]
        public string HotelName { get; set; }
        [Required]
        public string Description { get; set; }
        [MaxLength(20)]
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [MaxLength(300)]
        public string Address { get; set; }  // (City,Country)
        public int StarRating { get; set; } // 1-5 Stars
        public string MainImageUrl { get; set; }

        // Owner from Identity User
        [Required]
        public string OwnerId { get; set; }
        public ApplicationUser? Owner { get; set; }

// Navigation
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<HotelImage> HotelImages { get; set; } = new List<HotelImage>();

    }
}
