using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hotel_Managment.Models;

public class Room
{
    [Key]
    public int RoomId { get; set; }

    [Required]
    public string RoomNumber { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PricePerNight { get; set; }

    [Required]
    public bool IsAvailable { get; set; } = true;

    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public int RoomTypeId { get; set; }
    public RoomType? RoomType { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
}
