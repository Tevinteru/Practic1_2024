using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practic1_2024.Data;
using Practic1_2024.Models;

namespace Practic1_2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmartphoneCharacteristicsController : ControllerBase
    {
        private readonly StoreDbContext _context;

        public SmartphoneCharacteristicsController(StoreDbContext context)
        {
            _context = context;
        }

        // GET: api/SmartphoneCharacteristics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SmartphoneCharacteristic>>> GetSmartphoneCharacteristics()
        {
            return await _context.SmartphoneCharacteristics
                .Include(sc => sc.Smartphone) // Включаем информацию о смартфоне
                .ToListAsync();
        }

        // GET: api/SmartphoneCharacteristics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SmartphoneCharacteristic>> GetSmartphoneCharacteristic(int id)
        {
            var characteristic = await _context.SmartphoneCharacteristics
                .Include(sc => sc.Smartphone) // Включаем информацию о смартфоне
                .FirstOrDefaultAsync(sc => sc.Id == id);

            if (characteristic == null)
            {
                return NotFound();
            }

            return characteristic;
        }

        // PUT: api/SmartphoneCharacteristics/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSmartphoneCharacteristic(int id, SmartphoneCharacteristic characteristic)
        {
            if (id != characteristic.Id)
            {
                return BadRequest();
            }

            _context.Entry(characteristic).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SmartphoneCharacteristicExists(id))
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

        // POST: api/SmartphoneCharacteristics
        [HttpPost]
        public async Task<ActionResult<SmartphoneCharacteristic>> PostSmartphoneCharacteristic(SmartphoneCharacteristic characteristic)
        {
            _context.SmartphoneCharacteristics.Add(characteristic);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSmartphoneCharacteristic), new { id = characteristic.Id }, characteristic);
        }

        // DELETE: api/SmartphoneCharacteristics/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSmartphoneCharacteristic(int id)
        {
            var characteristic = await _context.SmartphoneCharacteristics.FindAsync(id);
            if (characteristic == null)
            {
                return NotFound();
            }

            _context.SmartphoneCharacteristics.Remove(characteristic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SmartphoneCharacteristicExists(int id)
        {
            return _context.SmartphoneCharacteristics.Any(e => e.Id == id);
        }
    }
}
