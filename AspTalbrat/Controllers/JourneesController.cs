using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspTalbrat.Models;

namespace AspTalbrat.Controllers
{
    public class JourneesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JourneesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Journees
        public async Task<IActionResult> Index()
        {
              return _context.Journees != null ? 
                          View(await _context.Journees.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Journees'  is null.");
        }

        // GET: Journees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Journees == null)
            {
                return NotFound();
            }

            var journee = await _context.Journees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journee == null)
            {
                return NotFound();
            }

            return View(journee);
        }

        // GET: Journees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Journees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Numero,Note")] Journee journee)
        {
            //using var bodyReader = new StreamReader(HttpContext.Request.Body);
            
            return Ok(HttpContext.Request.Body);
            /*
            if (ModelState.IsValid)
            
                _context.Add(journee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(journee);
            */
        }

        // GET: Journees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Journees == null)
            {
                return NotFound();
            }

            var journee = await _context.Journees.FindAsync(id);
            if (journee == null)
            {
                return NotFound();
            }
            return View(journee);
        }

        // POST: Journees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Numero,Note")] Journee journee)
        {
            if (id != journee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(journee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JourneeExists(journee.Id))
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
            return View(journee);
        }

        // GET: Journees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Journees == null)
            {
                return NotFound();
            }

            var journee = await _context.Journees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journee == null)
            {
                return NotFound();
            }

            return View(journee);
        }

        // POST: Journees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Journees == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Journees'  is null.");
            }
            var journee = await _context.Journees.FindAsync(id);
            if (journee != null)
            {
                _context.Journees.Remove(journee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JourneeExists(int id)
        {
          return (_context.Journees?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
