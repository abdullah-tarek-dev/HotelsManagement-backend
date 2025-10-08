using Npgsql;

namespace Hotel_Managment.DTOs
{
    public class TopHotelDto
    {
        public int Hotel_Id { get; set; }
        public string Hotel_Name { get; set; }
        public long Total_Bookings { get; set; }
}


    }
