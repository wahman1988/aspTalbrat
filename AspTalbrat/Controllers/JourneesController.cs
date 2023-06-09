using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspTalbrat.Models;
using System.Drawing.Printing;
using System.Drawing;

namespace AspTalbrat.Controllers
{
    public class JourneesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PrintDocument printDoc = new PrintDocument();
        private readonly IConfiguration _configuration;
        private Journee selectedJournee;

        public JourneesController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

            var journee = await _context.Journees.Include(p => p.Paiements)
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
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create([FromBody] JourneeViewModel data)
        {
            //using var bodyReader = new StreamReader(HttpContext.Request.Body);

            if (!ModelState.IsValid)
                return new JsonResult(new { errors = "errors", details = ModelState });
            if (ModelState.IsValid)
            {
                var journee = new Journee
                {
                    Date = data.Date,
                    Numero = data.Numero,
                    Note = data.Note,
                };
                _context.Journees.Add(journee);
                await _context.SaveChangesAsync();

                foreach (var paiement in data.Paiements)
                {
                    _context.Paiements.Add(new JourneePaiement
                    {
                        JourneeId = journee.Id,
                        Libelle = paiement.Libelle,
                        Montant = paiement.Montant
                    });
                    await _context.SaveChangesAsync();
                }

                return new JsonResult("ok");
            }
            return new JsonResult(data);


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

        public async Task<IActionResult> PrintJournee(int id)
        {
            var printer = _configuration.GetValue<String>("Printer");
            // Set the document to print to the default printer
            //  printDoc.PrinterSettings.PrinterName = PrinterSettings.InstalledPrinters[0];
            printDoc.PrinterSettings.PrinterName = printer;
            // Add an event handler to print the invoice
            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);


            //string wwwrootPath = _env.WebRootPath;
            if (id == null || _context.Journees == null)
            {
                return NotFound();
            }

            var journee = await _context.Journees.Include(j => j.Paiements).FirstOrDefaultAsync(a => a.Id == id);
            if (journee == null)
            {
                return NotFound();
            }
            selectedJournee = journee;




            // Print the invoice
            printDoc.Print();
            return RedirectToAction("Index", "Journees");

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
            e.Graphics.DrawString($"REF : {selectedJournee.Id}", new Font("Arial", 12), Brushes.Black, new PointF(0, 40));
            e.Graphics.DrawString($"DATE : {selectedJournee.Date.ToString("dd/MM/yyyy")}", new Font("Arial", 12), Brushes.Black, new PointF(0, 80));
           // e.Graphics.DrawString($"NOM  : {selectedJournee.Employee.Name}", new Font("Arial", 12), Brushes.Black, new PointF(0, 120));
            //e.Graphics.DrawString($"MONTANT  : {selectedJournee.Montant.ToString("#,##0.00")}", new Font("Arial", 12), Brushes.Black, new PointF(0, 160));
            e.Graphics.DrawString($"NOTES  : {selectedJournee.Note}", new Font("Arial", 12), Brushes.Black, new PointF(0, 200));
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
