using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TallerBackend.Models
{
    public class Servicio
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Nombre { get; set; }

        [Required, MaxLength(1000)]
        public string Descripcion { get; set; }

        [Required, MaxLength(500)]
        public string ImagenUrl { get; set; }
    }
}
