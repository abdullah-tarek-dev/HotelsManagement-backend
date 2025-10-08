using System.ComponentModel.DataAnnotations;

namespace Hotel_Managment.DTOs
{
    public class HotelImageUploadDto
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public int HotelId { get; set; }

        public string? Caption { get; set; }

        public bool IsMain { get; set; } = false;
    }

}
