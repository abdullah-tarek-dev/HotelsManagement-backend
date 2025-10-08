using Hotel_Managment.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hotel_Managment.Data
{
    public class HotelDbContextFactory : IDesignTimeDbContextFactory<HotelDbContext>
    {
        public HotelDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HotelDbContext>();

            // 🔹 هنا تحط نفس connection string اللي عندك في appsettings.json
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=HotelManagment;Username=postgres;Password=123456");

            return new HotelDbContext(optionsBuilder.Options);
        }
    }
}
