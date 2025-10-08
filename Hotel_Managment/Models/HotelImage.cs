using System.ComponentModel.DataAnnotations;

namespace Hotel_Managment.Models
{
    public class HotelImage
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; }  // نخزن رابط الصورة أو Path في السيرفر

        public string? Caption { get; set; }  // وصف اختياري للصورة
        public bool IsMain { get; set; } = false;  // صورة رئيسية للفندق

        // FK
        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; } 
    }
}
