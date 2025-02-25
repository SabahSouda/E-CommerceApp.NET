using Microsoft.AspNetCore.Mvc;
using MonParfumApp.Models;
using MonParfumApp.Services;

namespace MonParfumApp.Controllers
{
    [Route("categories")]
    public class CategorieController : Controller
    {
        private readonly CategorieService _categorieService;

        public CategorieController(CategorieService categorieService)
        {
            _categorieService = categorieService;
        }

        // Action pour afficher la liste des catégories (Vue)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _categorieService.GetAllCategoriesAsync();
            return View(categories); // Vue Index
        }

        // Action pour afficher la vue de création (GET)
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        // Action pour créer une nouvelle catégorie (POST)
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categorie categorie)
        {
            if (ModelState.IsValid)
            {
                await _categorieService.AddCategorieAsync(categorie);
                return RedirectToAction(nameof(Index)); // Rediriger vers la liste après création
            }
            return View(categorie); // Retourner la vue si la validation échoue
        }

        // Action pour afficher la vue d'édition (GET)
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var categorie = await _categorieService.GetCategorieByIdAsync(id);
            if (categorie == null)
            {
                return NotFound();
            }
            return View(categorie); // Vue Edit
        }

        // Action pour mettre à jour une catégorie (POST)
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categorie categorie)
        {
            if (id != categorie.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _categorieService.UpdateCategorieAsync(categorie);
                return RedirectToAction(nameof(Index)); // Rediriger vers la liste après modification
            }
            return View(categorie); // Retourner la vue si la validation échoue
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var categorie = await _categorieService.GetCategorieByIdAsync(id);
            if (categorie == null)
            {
                // Si la catégorie n'existe pas, retourner une réponse 404 (Non trouvé)
                return NotFound();
            }

            // Supprimer la catégorie
            await _categorieService.DeleteCategorieAsync(id);

            // Confirmer que la suppression a réussi et rediriger vers la vue des catégories
            TempData["SuccessMessage"] = "La catégorie a été supprimée avec succès.";
            return RedirectToAction(nameof(Index));
        }




    }
}
