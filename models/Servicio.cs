using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TallerBackend.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(1000)]
        public string Descripcion { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string ImagenUrl { get; set; } = string.Empty;

        // ✅ Quita el Column(TypeName) — SQLite lo guarda como REAL/TEXT igualmente
        public decimal Precio { get; set; }
    }
}