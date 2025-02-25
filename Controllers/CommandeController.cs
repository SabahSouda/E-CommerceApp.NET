using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonParfumApp.Data;
using MonParfumApp.Models;
using MonParfumApp.Services;

namespace MonParfumApp.Controllers
{
    public class CommandeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ParfumService _parfumService; // Injection directe de ParfumService

        public CommandeController(ApplicationDbContext context, ParfumService parfumService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _parfumService = parfumService ?? throw new ArgumentNullException(nameof(parfumService));
        }


        // Action pour afficher toutes les commandes
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Récupérer toutes les commandes de la base de données
            var commandes = await _context.Commandes
                .Include(c => c.Items)
                .ThenInclude(i => i.Parfum)
                .Include(c => c.Client)
                .ToListAsync();

            return View(commandes); // Ceci affichera la vue Commande/Index
        }


        // Action pour passer une commande
        [HttpPost]
        public async Task<IActionResult> Commande(int id, int quantite)
        {
            // Vérification de l'identifiant du parfum
            if (id <= 0)
            {
                return BadRequest("Identifiant de parfum invalide.");
            }

            // Vérification de la quantité
            if (quantite <= 0)
            {
                TempData["ErrorMessage"] = "La quantité doit être supérieure à zéro.";
                return RedirectToAction("Index", "Parfum");
            }

            var parfum = await _parfumService.GetParfumByIdAsync(id);
            if (parfum == null)
            {
                return NotFound("Le parfum demandé n'existe pas.");
            }

            // Vérification de l'utilisateur authentifié
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(clientId))
            {
                return Unauthorized("Utilisateur non authentifié.");
            }

            // Création de la commande
            var commande = new Commande
            {
                DateCommande = DateTime.Now,
                ClientId = clientId,
                Total = parfum.Prix * quantite, // Calcul du total en fonction de la quantité
                Items = new List<CommandeItem>
        {
            new CommandeItem
            {
                ParfumId = parfum.Id,
                Quantite = quantite,
                PrixUnitaire = parfum.Prix
            }
        }
            };

            // Enregistrement de la commande dans la base de données
            _context.Commandes.Add(commande);
            await _context.SaveChangesAsync(); // Utilisation de SaveChangesAsync pour la persistance asynchrone

            // Message de confirmation
            TempData["SuccessMessage"] = "Commande ajoutée avec succès !";

            // Redirection vers la page des parfums
            return RedirectToAction("Index", "Parfum");
        }



    }
}
