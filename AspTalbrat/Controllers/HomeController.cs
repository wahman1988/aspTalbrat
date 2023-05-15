using AspTalbrat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;

namespace AspTalbrat.Controllers
{
	[Authorize]
	public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly PrintDocument printDoc = new PrintDocument();
        private readonly ApplicationDbContext _context;
        private  Product  selectedProduct;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env, IConfiguration configuration, ApplicationDbContext context)
        {
            _logger = logger;
            _env = env;
            _configuration = configuration;
            _context = context;
        }

   
        public async Task<IActionResult> Order(int id)
        {
           //string wwwrootPath = _env.WebRootPath;
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            selectedProduct = product;
            //filePath = wwwrootPath + "/" + product.ImageUrl;

            //filePath = "/img/cafe.jpg";

            var printer = _configuration.GetValue<String>("Printer");
            // Set the document to print to the default printer
            //  printDoc.PrinterSettings.PrinterName = PrinterSettings.InstalledPrinters[0];
            printDoc.PrinterSettings.PrinterName = printer;
            // Add an event handler to print the invoice
           printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
         




            // Print the invoice
            printDoc.Print();
            return Redirect("/");
            

        }

       
        public async Task<IActionResult> Index()
        {
            
            return View(await _context.Products.ToListAsync());
       
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        private void PrintPage(object sender, PrintPageEventArgs e)
        {
       

            string wwwrootPath = _env.WebRootPath;
            var filePath = wwwrootPath + "/" + selectedProduct.ImageUrl;

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
            e.Graphics.DrawString(selectedProduct.Name, f14, Brushes.Black, centermargin, 3, center);

            Graphics g = e.Graphics;

            // Load the image to be printed
            Image img = Image.FromFile(filePath);

            // Draw the image on the page
            g.DrawImage(img, new Rectangle(65, 40, img.Width, img.Height));

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
