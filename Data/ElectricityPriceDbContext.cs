using Microsoft.EntityFrameworkCore;
using Microservice_Teht.Models;

namespace Microservice_Teht.Data
{
    public class ElectricityPriceDbContext : DbContext
    {
        public DbSet<ElectricityPriceInfo> ElectricityPrices { get; set; }

        public ElectricityPriceDbContext(DbContextOptions<ElectricityPriceDbContext> options) : base(options)
        {
        }
    }
}
