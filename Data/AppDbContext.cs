using TallerBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace TallerBackend.Data
{
   public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Producto> Productos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>()
            .Property(p => p.Precio)
            .HasConversion<double>(); // ✅ SQLite entiende double, no decimal nativo
    }
}
}
