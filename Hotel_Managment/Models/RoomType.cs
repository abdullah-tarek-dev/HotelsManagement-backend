using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Hotel_Managment.Models
{
    public class RoomType
    {
        [Key]
        public int RoomTypeId { get; set; }
        [Required]
        [MaxLength(200)]
        public string RoomName { get; set; }
        [Required]
        [MaxLength(400)]
        public string Description { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }


        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
