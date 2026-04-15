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

        public DbSet<Servicio> Servicios { get; set; }
    }
}
