using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MonParfumApp.Models;

namespace MonParfumApp.Controllers
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public IActionResult AccessDenied(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            TempData["ErrorMessage"] = "Vous n'avez pas les autorisations nécessaires pour accéder à cette page.";
            return View(); // Affiche la vue AccessDenied
        }


    }
}
