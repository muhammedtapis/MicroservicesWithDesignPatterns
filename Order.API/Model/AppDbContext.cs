using Microsoft.EntityFrameworkCore;

namespace Order.API.Model
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Orderr> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}