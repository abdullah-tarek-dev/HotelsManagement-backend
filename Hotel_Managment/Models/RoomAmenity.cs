using System.ComponentModel.DataAnnotations;

namespace Hotel_Managment.Models
{
    public class RoomAmenity
    {
        public int RoomId { get; set; }
        public Room? Room { get; set; }

        public int AmenityId { get; set; }
        public Amenity? Amenity { get; set; }
    }
}
