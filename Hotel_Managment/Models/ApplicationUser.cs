using Microsoft.AspNetCore.Identity;
namespace Hotel_Managment.Models
{
    public class ApplicationUser:IdentityUser
    {
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        //public string FullName { get; set; } 

        public string Country { get; set; } 

        public string City {  get; set; } = string.Empty;
    }
}
