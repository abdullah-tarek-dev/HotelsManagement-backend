using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Managment.Models
{
    public class Discount
    {
        [Key]
        public int DistId { get; set; }
        public string Code { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DisValue { get; set; }
        public bool IsPercentage { get; set; }
        public bool IsActive { get; set; }
        public int UsageLimit { get; set; }
        public DateTime ExpiryDate { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
