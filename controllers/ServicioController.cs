using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerBackend.Data;
using TallerBackend.Models;

namespace TallerBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServicioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Servicio>>> GetServicios()
        {
            return await _context.Servicios.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Servicio>> GetServicio(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null) return NotFound();
            return servicio;
        }

        [HttpPost]
        public async Task<ActionResult<Servicio>> PostServicio(Servicio servicio)
        {
            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetServicio), new { id = servicio.Id }, servicio);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutServicio(int id, Servicio servicio)
        {
            if (id != servicio.Id) return BadRequest();
            _context.Entry(servicio).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServicio(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio == null) return NotFound();
            _context.Servicios.Remove(servicio);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
