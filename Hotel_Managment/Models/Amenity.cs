using System.ComponentModel.DataAnnotations;

namespace Hotel_Managment.Models
{
    public class Amenity
    {
        [Key]
        public int AmenityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
    }
}

