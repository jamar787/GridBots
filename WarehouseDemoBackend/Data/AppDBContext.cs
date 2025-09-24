using Microsoft.EntityFrameworkCore;
using WarehouseDemoBackend.Models;

namespace WarehouseDemoBackend.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Coordinate> Coordinates { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            mb.Entity<Coordinate>()
                .HasKey(c => c.Id);
            mb.Entity<Coordinate>()
                .Property(c => c.UpdatedAt)
                .HasDefaultValueSql("now()");
        }
    }
}
