using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartPark.Data;
using SmartPark.Models;
using SmartPark.Filters;

namespace SmartPark.Controllers_Api
{
    [Route("api/v1/placila")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class PlaciloApiController : ControllerBase
    {
        private readonly SmartParkContext _context;

        public PlaciloApiController(SmartParkContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Placilo>>> GetPlacilo()
        {
            return await _context.Placila.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Placilo>> GetPlacilo(int id)
        {
            var placilo = await _context.Placila.FindAsync(id);

            if (placilo == null)
                return NotFound();

            return placilo;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlacilo(int id, Placilo placilo)
        {
            if (id != placilo.Id)
                return BadRequest();

            _context.Entry(placilo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Placila.AnyAsync(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Placilo>> PostPlacilo(Placilo placilo)
        {
            _context.Placila.Add(placilo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlacilo), new { id = placilo.Id }, placilo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlacilo(int id)
        {
            var placilo = await _context.Placila.FindAsync(id);
            if (placilo == null)
                return NotFound();

            _context.Placila.Remove(placilo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}