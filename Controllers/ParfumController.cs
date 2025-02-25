using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonParfumApp.Models;
using MonParfumApp.Services;
using Microsoft.AspNetCore.Hosting;  // Pour IWebHostEnvironment
using Microsoft.Extensions.Hosting;  // Pour IHostEnvironment (si nécessaire)

namespace MonParfumApp.Controllers
{
    [Authorize] // L'utilisateur doit être authentifié pour accéder à ce contrôleur
    public class ParfumController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment; // Injection de IWebHostEnvironment
        private readonly ParfumService _parfumService;
        private readonly CategorieService _categorieService;

        public ParfumController(ParfumService parfumService, CategorieService categorieService, IWebHostEnvironment webHostEnvironment)
        {
            _parfumService = parfumService;
            _categorieService = categorieService;
            _webHostEnvironment = webHostEnvironment; // Initialisation de _webHostEnvironment
        }

        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var parfums = await _parfumService.GetAllParfumsAsync();
                return View(parfums); // Une liste de parfums est passée à la vue
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erreur lors de la récupération des parfums : {ex.Message}");
                return View("Error");
            }
        }

        // Action GET pour afficher le formulaire de création d'un parfum
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Categories = await _categorieService.GetAllCategoriesAsync();
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erreur lors de la récupération des catégories : {ex.Message}");
                return View("Error"); // Vue d'erreur générique
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Parfum parfum)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categorieService.GetAllCategoriesAsync();
                return View(parfum);
            }

            try
            {
                if (parfum.ImageFile != null)
                {
                    // Définir le chemin de sauvegarde
                    var fileName = Path.GetFileName(parfum.ImageFile.FileName);
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images/parfums");

                    // Créer le dossier si nécessaire
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var filePath = Path.Combine(uploadPath, fileName);

                    // Sauvegarder le fichier
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await parfum.ImageFile.CopyToAsync(stream);
                    }

                    // Mettre à jour le chemin de l'image
                    parfum.ImagePath = $"/images/parfums/{fileName}";
                }

                // Ajouter le parfum à la base de données
                await _parfumService.AddParfumAsync(parfum);

                TempData["SuccessMessage"] = "Le parfum a été ajouté avec succès.";
                return RedirectToAction("ParfumIndex", "Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Une erreur s'est produite : {ex.Message}");
                ViewBag.Categories = await _categorieService.GetAllCategoriesAsync();
                return View(parfum);
            }
        }

       
        // GET: /Parfum/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var parfum = await _parfumService.GetParfumByIdAsync(id);
            if (parfum == null)
            {
                return NotFound("Le parfum spécifié n'existe pas.");
            }

            ViewBag.Categories = await _categorieService.GetAllCategoriesAsync(); // Charger les catégories pour le dropdown
            return View(parfum);
        }


        // POST: /Parfum/Edit/{id}
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Parfum parfum, IFormFile? imageFile)
        {
            if (id != parfum.Id)
            {
                return BadRequest("Les identifiants ne correspondent pas.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categorieService.GetAllCategoriesAsync();
                return View(parfum);
            }

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Supprimer l'image existante si nécessaire
                    if (!string.IsNullOrEmpty(parfum.ImagePath))
                    {
                        var existingImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", parfum.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(existingImagePath))
                        {
                            System.IO.File.Delete(existingImagePath); // Supprimer l'ancienne image
                        }
                    }

                    // Définir le chemin d'enregistrement de la nouvelle image
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/parfums");

                    // Créer le dossier si nécessaire
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var filePath = Path.Combine(uploadPath, fileName);

                    // Sauvegarder le fichier
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // Mettre à jour l'ImagePath
                    parfum.ImagePath = $"/images/parfums/{fileName}";
                }

                // Mettre à jour le parfum
                await _parfumService.UpdateParfumAsync(parfum);
                TempData["SuccessMessage"] = "Le parfum a été mis à jour avec succès.";
                return RedirectToAction("ParfumIndex", "Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Categories = await _categorieService.GetAllCategoriesAsync();
                return View(parfum);
            }
        }




        // Supprimer Un parfum 
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var parfum = await _parfumService.GetParfumByIdAsync(id);
                if (parfum == null)
                {
                    return NotFound("Le parfum spécifié n'existe pas.");
                }

                await _parfumService.DeleteParfumAsync(id);
                TempData["SuccessMessage"] = "Le parfum a été supprimé avec succès.";
                return RedirectToAction("ParfumIndex", "Admin");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Une erreur s'est produite : {ex.Message}";
                return RedirectToAction("ParfumIndex", "Admin");
            }
        }
    }
}
