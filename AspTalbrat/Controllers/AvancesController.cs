﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspTalbrat.Models;

using System.Drawing;
using System.Drawing.Printing;
using Microsoft.Extensions.Logging;
using System.Configuration;
using AspTalbrat.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace AspTalbrat.Controllers
{
    public class AvancesController : Controller
    {
        private readonly PrintDocument printDoc = new PrintDocument();
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;
        private Avance  selectedAvance;

        public AvancesController(IWebHostEnvironment env, IConfiguration configuration, ApplicationDbContext context)
        {
            _context = context;
     
            _env = env;
            _configuration = configuration;
        
        }

        // GET: Avances
        public async Task<IActionResult> Index()
        {
            var  total = _context.Avances.Sum(a => (float)a.Montant);
            /*
            decimal total = 0;
            foreach (Avance item in _context.Avances.ToList()) {
                total = item.Montant + total;
            }
            */
            ViewData["Total"] = total.ToString("# ##0.00"); 
            var applicationDbContext = _context.Avances.Include(a => a.Employee);
            return View(await applicationDbContext.OrderByDescending(a=>a.Date).ThenByDescending(a => a.Id).ToListAsync());
        }

        // GET: Avances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Avances == null)
            {
                return NotFound();
            }

            var avance = await _context.Avances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (avance == null)
            {
                return NotFound();
            }

            return View(avance);
        }


        public IActionResult Etat()
        {
            List<EtatViewModel> etat = new List<EtatViewModel>();
            foreach (Employee item in _context.Employees.ToList())
            {
                var total = _context.Avances.Where(emp=>emp.EmployeeId == item.Id).Sum(a => (float)a.Montant);
                etat.Add(new EtatViewModel
                {
                       employee = item.Name,
                       total =  total.ToString("#,##0.00")

                });
            }
            etat = etat.OrderByDescending(i=> float.Parse(i.total)).ToList();
           // var total = _context.Avances.Sum(a => (float)a.Montant);
            /*
            decimal total = 0;
            foreach (Avance item in _context.Avances.ToList()) {
                total = item.Montant + total;
            }
            */
            
            var applicationDbContext = _context.Avances.Include(a => a.Employee);
            return View(etat);
        }
        // GET: Avances/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name");
            return View();
        }

        // POST: Avances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Note,Montant,EmployeeId,Date")] Avance avance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(avance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id", avance.EmployeeId);
            return View(avance);
        }

		// GET: Avances/Edit/5
		[Authorize]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Avances == null)
            {
                return NotFound();
            }

            var avance = await _context.Avances.FindAsync(id);
            if (avance == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", avance.EmployeeId);
            return View(avance);
        }

		// POST: Avances/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize]
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Note,Montant,EmployeeId,Date")] Avance avance)
        {
            if (id != avance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(avance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvanceExists(avance.Id))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id", avance.EmployeeId);
            return View(avance);
        }

		// GET: Avances/Delete/5
		[Authorize]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Avances == null)
            {
                return NotFound();
            }

            var avance = await _context.Avances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (avance == null)
            {
                return NotFound();
            }

            return View(avance);
        }

		[Authorize]
		// POST: Avances/Delete/5
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Avances == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Avances'  is null.");
            }
            var avance = await _context.Avances.FindAsync(id);
            if (avance != null)
            {
                _context.Avances.Remove(avance);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AvanceExists(int id)
        {
          return (_context.Avances?.Any(e => e.Id == id)).GetValueOrDefault();
        }




      
        public async Task<IActionResult> PrintAvance(int id)
        {
            var printer = _configuration.GetValue<String>("Printer");
            // Set the document to print to the default printer
            //  printDoc.PrinterSettings.PrinterName = PrinterSettings.InstalledPrinters[0];
            printDoc.PrinterSettings.PrinterName = printer;
            // Add an event handler to print the invoice
            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);

            
            //string wwwrootPath = _env.WebRootPath;
            if (id == null || _context.Avances == null)
            {
                return NotFound();
            }

            var avance = await _context.Avances.Include(a => a.Employee).FirstOrDefaultAsync(a => a.Id == id);
            if (avance == null)
            {
                return NotFound();
            }
            selectedAvance = avance;


          

            // Print the invoice
            printDoc.Print();
            return RedirectToAction("Index", "Avances");
       
        }
        private void PrintPage(object sender, PrintPageEventArgs e)
        {
       


            var f8 = new Font("Calibri", 8, FontStyle.Regular);
            var f10 = new Font("Calibri", 10, FontStyle.Regular);
            var f10b = new Font("Calibri", 10, FontStyle.Bold);
            var f14 = new Font("Calibri", 14, FontStyle.Bold);



            int leftmargin = printDoc.DefaultPageSettings.Margins.Left;
            int centermargin = printDoc.DefaultPageSettings.PaperSize.Width / 2;
            int rightmargin = printDoc.DefaultPageSettings.PaperSize.Width;

            StringFormat right = new StringFormat();
            StringFormat center = new StringFormat();
       

            right.Alignment = StringAlignment.Far;
            center.Alignment = StringAlignment.Center;
           

            string line;

            line = "---------------------------------------------------------------------------------";

            //e.Graphics.DrawString("DATE : ", f14, Brushes.Black, leftmargin, 3, center);
            //e.Graphics.DrawString("NOM  : ", f14, Brushes.Black, leftmargin, 25, center);
            e.Graphics.DrawString("BON D'AVANCE", f14, Brushes.Black, centermargin, 3, center);
            e.Graphics.DrawString($"REF : {selectedAvance.Id}", new Font("Arial", 12), Brushes.Black, new PointF(0, 40));
            e.Graphics.DrawString($"DATE : {selectedAvance.Date.ToString("dd/MM/yyyy")}", new Font("Arial", 12), Brushes.Black, new PointF(0, 80));
            e.Graphics.DrawString($"NOM  : {selectedAvance.Employee.Name}", new Font("Arial", 12), Brushes.Black, new PointF(0, 120));
            e.Graphics.DrawString($"MONTANT  : {selectedAvance.Montant.ToString("#,##0.00")}", new Font("Arial", 12), Brushes.Black, new PointF(0, 160));
            e.Graphics.DrawString($"NOTES  : {selectedAvance.Note}", new Font("Arial", 12), Brushes.Black, new PointF(0, 200));
            e.Graphics.DrawString("SIGNATURE", f14, Brushes.Black, centermargin, 240, center);
            e.Graphics.DrawString("************", f14, Brushes.Black, centermargin, 340, center);



            /*
            var f8 = new Font("Calibri", 8, FontStyle.Regular);
            var f10 = new Font("Calibri", 10, FontStyle.Regular);
            var f10b = new Font("Calibri", 10, FontStyle.Bold);
            var f14 = new Font("Calibri", 14, FontStyle.Bold);


            int leftmargin = printDoc.DefaultPageSettings.Margins.Left;
            int centermargin = printDoc.DefaultPageSettings.PaperSize.Width / 2;
            int rightmargin = printDoc.DefaultPageSettings.PaperSize.Width;

            StringFormat right = new StringFormat();
            StringFormat center = new StringFormat();

            right.Alignment = StringAlignment.Far;
            center.Alignment = StringAlignment.Center;

            string line;

            line = "---------------------------------------------------------------------------------";
            e.Graphics.DrawString("CAFE & REST BANDL", f14, Brushes.Black, centermargin, 3, center);
            e.Graphics.DrawString("STATION WINXO IMINTANOUT", f10, Brushes.Black, centermargin, 25, center);
            e.Graphics.DrawString("06 67 96 35 53", f10, Brushes.Black, centermargin, 40, center);
     

            int height = 0;
            long i;

            string qte;
            string total;
            double countprice = 0;

            e.Graphics.DrawString("Qte", f10, Brushes.Black, 0, 100);
            e.Graphics.DrawString("Article", f10, Brushes.Black, 50, 100);
            // e.Graphics.DrawString("Total", f10, Brushes.Black, 180, 80);
            e.Graphics.DrawString("Total", f10, Brushes.Black, rightmargin, 100, right);
            */

        }
    }
}
