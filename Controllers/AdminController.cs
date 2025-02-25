using Microsoft.AspNetCore.Mvc;
using MonParfumApp.Services;  // Assurez-vous d'importer les services nécessaires

namespace MonParfumApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ParfumService _parfumService; // Ajouter l'injection de dépendance pour ParfumService

        // Injection du service ParfumService via le constructeur
        public AdminController(ParfumService parfumService)
        {
            _parfumService = parfumService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ParfumIndex()
        {
            // Utilisation du service pour récupérer les parfums
            var parfums = await _parfumService.GetAllParfumsAsync();
            return View(parfums); // Ceci affichera la vue Admin/ParfumIndex
        }


    }
}
