using System.Diagnostics;
using AgriEnergy.Models;
using Microsoft.AspNetCore.Mvc;

namespace AgriEnergy.Controllers
{ 
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult SustainableFarmingHub()
        {
            return View();
        }

        public IActionResult GreenEnergyMarketplace()
        {
            return View();
        }

        public IActionResult EducationalResources()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
