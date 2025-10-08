using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Managment.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public DateTime PayDate { get; set; }
        public string PayMethod { get; set; }
        public string TransactionId { get; set; }
        public string PayStatus { get; set; }

        public int BookingId { get; set; }
        public Booking? Booking { get; set; }
    }
}
