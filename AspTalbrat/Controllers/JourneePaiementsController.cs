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
    public class JourneePaiementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JourneePaiementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: JourneePaiements
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Paiements.Include(j => j.Journee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: JourneePaiements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Paiements == null)
            {
                return NotFound();
            }

            var journeePaiement = await _context.Paiements
                .Include(j => j.Journee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journeePaiement == null)
            {
                return NotFound();
            }

            return View(journeePaiement);
        }

        // GET: JourneePaiements/Create
        public IActionResult Create()
        {
            ViewData["JourneeId"] = new SelectList(_context.Journees, "Id", "Id");
            return View();
        }

        // POST: JourneePaiements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Libelle,JourneeId,Montant")] JourneePaiement journeePaiement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(journeePaiement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["JourneeId"] = new SelectList(_context.Journees, "Id", "Id", journeePaiement.JourneeId);
            return View(journeePaiement);
        }

        // GET: JourneePaiements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Paiements == null)
            {
                return NotFound();
            }

            var journeePaiement = await _context.Paiements.FindAsync(id);
            if (journeePaiement == null)
            {
                return NotFound();
            }
            ViewData["JourneeId"] = new SelectList(_context.Journees, "Id", "Id", journeePaiement.JourneeId);
            return View(journeePaiement);
        }

        // POST: JourneePaiements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Libelle,JourneeId,Montant")] JourneePaiement journeePaiement)
        {
            if (id != journeePaiement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(journeePaiement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JourneePaiementExists(journeePaiement.Id))
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
            ViewData["JourneeId"] = new SelectList(_context.Journees, "Id", "Id", journeePaiement.JourneeId);
            return View(journeePaiement);
        }

        // GET: JourneePaiements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Paiements == null)
            {
                return NotFound();
            }

            var journeePaiement = await _context.Paiements
                .Include(j => j.Journee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journeePaiement == null)
            {
                return NotFound();
            }

            return View(journeePaiement);
        }

        // POST: JourneePaiements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Paiements == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Paiements'  is null.");
            }
            var journeePaiement = await _context.Paiements.FindAsync(id);
            if (journeePaiement != null)
            {
                _context.Paiements.Remove(journeePaiement);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JourneePaiementExists(int id)
        {
          return (_context.Paiements?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
