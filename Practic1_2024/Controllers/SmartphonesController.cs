using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Practic1_2024.Data;
using Practic1_2024.Models;

namespace Practic1_2024.Controllers
{
    public class SmartphonesController : Controller
    {
        private readonly StoreDbContext _context;

        public SmartphonesController(StoreDbContext context)
        {
            _context = context;
        }

        // GET: Smartphones
        public async Task<IActionResult> Index()
        {
            var storeDbContext = _context.Smartphones.Include(s => s.Category);
            return View(await storeDbContext.ToListAsync());
        }

        // GET: Smartphones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var smartphone = await _context.Smartphones
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (smartphone == null)
            {
                return NotFound();
            }

            return View(smartphone);
        }

        // GET: Smartphones/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");

            return View();
        }

        // POST: Smartphones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Manufacturer,CategoryId,QuantityInStock,Image,DateAdded")] Smartphone smartphone)
        {
            Console.WriteLine("Сука");
            if (ModelState.IsValid)
            {
                Console.WriteLine("Пошел нахуй");

                smartphone.OrderItems = new List<OrderItem>();

                _context.Add(smartphone);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    // Выводим все ошибки, чтобы понять, что именно не так
                    Console.WriteLine(error.ErrorMessage);
                }
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", smartphone.CategoryId);
                return View(smartphone);
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", smartphone.CategoryId);
            Console.WriteLine("ХУя");

            return View(smartphone);
        }


        // GET: Smartphones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var smartphone = await _context.Smartphones.FindAsync(id);
            if (smartphone == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", smartphone.CategoryId);
            return View(smartphone);
        }

        // POST: Smartphones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Manufacturer,CategoryId,QuantityInStock,Image,DateAdded")] Smartphone smartphone)
        {
            if (id != smartphone.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(smartphone);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SmartphoneExists(smartphone.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", smartphone.CategoryId);
            return View(smartphone);
        }

        // GET: Smartphones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var smartphone = await _context.Smartphones
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (smartphone == null)
            {
                return NotFound();
            }

            return View(smartphone);
        }

        // POST: Smartphones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var smartphone = await _context.Smartphones.FindAsync(id);
            if (smartphone != null)
            {
                _context.Smartphones.Remove(smartphone);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SmartphoneExists(int id)
        {
            return _context.Smartphones.Any(e => e.Id == id);
        }
    }
}
