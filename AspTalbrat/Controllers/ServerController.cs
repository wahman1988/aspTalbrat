using AspTalbrat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AspTalbrat.Controllers
{
    public class ServerController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public ServerController(IWebHostEnvironment env, IConfiguration configuration)
        {
    
            _env = env;
            _configuration = configuration;
        
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RestartServer()
        {
            var apachePath = _configuration.GetValue<String>("ApachePath");
            restartHttpServer(apachePath);
            return RedirectToAction("Index", "Avances");
        }


        void restartHttpServer(string path)
        {
            Process[] pname = Process.GetProcessesByName("httpd");
            if (pname.Length == 0)
            {
                try
                {
                    using (Process myProcess = new Process())
                    {
                        myProcess.StartInfo.UseShellExecute = false;
                        // You can start any process, HelloWorld is a do-nothing example.
                        myProcess.StartInfo.FileName = path;
                        myProcess.StartInfo.CreateNoWindow = true;
                        myProcess.Start();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            else
            {
                foreach (var process in Process.GetProcessesByName("httpd"))
                {
                    process.Kill();
                }
                System.Threading.Thread.Sleep(2000);
                try
                {
                    using (Process myProcess = new Process())
                    {
                        myProcess.StartInfo.UseShellExecute = false;
                        // You can start any process, HelloWorld is a do-nothing example.
                        myProcess.StartInfo.FileName = "C:\\xampp\\apache\\bin\\httpd.exe";
                        myProcess.StartInfo.CreateNoWindow = true;
                        myProcess.Start();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }

        }
    }
}
