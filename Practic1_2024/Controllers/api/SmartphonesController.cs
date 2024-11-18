using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practic1_2024.Data;
using Practic1_2024.Models;

namespace Practic1_2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmartphonesController : ControllerBase
    {
        private readonly StoreDbContext _context;

        public SmartphonesController(StoreDbContext context)
        {
            _context = context;
        }

        // GET: api/Smartphones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Smartphone>>> GetSmartphones()
        {
            return await _context.Smartphones
                .Include(s => s.Brand)           // Включаем данные бренда
                .Include(s => s.Category)        // Включаем данные категории
                .Include(s => s.Characteristics) // Включаем характеристики смартфона
                .ToListAsync();
        }

        // GET: api/Smartphones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Smartphone>> GetSmartphone(int id)
        {
            var smartphone = await _context.Smartphones
                .Include(s => s.Brand)           // Включаем данные бренда
                .Include(s => s.Category)        // Включаем данные категории
                .Include(s => s.Characteristics) // Включаем характеристики
                .FirstOrDefaultAsync(s => s.Id == id);

            if (smartphone == null)
            {
                return NotFound();
            }

            return smartphone;
        }

        // PUT: api/Smartphones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSmartphone(int id, Smartphone smartphone)
        {
            if (id != smartphone.Id)
            {
                return BadRequest();
            }

            _context.Entry(smartphone).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SmartphoneExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Smartphones
        [HttpPost]
        public async Task<ActionResult<Smartphone>> PostSmartphone(Smartphone smartphone)
        {
            _context.Smartphones.Add(smartphone);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSmartphone), new { id = smartphone.Id }, smartphone);
        }

        // DELETE: api/Smartphones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSmartphone(int id)
        {
            var smartphone = await _context.Smartphones.FindAsync(id);
            if (smartphone == null)
            {
                return NotFound();
            }

            _context.Smartphones.Remove(smartphone);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SmartphoneExists(int id)
        {
            return _context.Smartphones.Any(e => e.Id == id);
        }
    }
}
