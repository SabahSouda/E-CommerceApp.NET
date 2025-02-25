using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonParfumApp.Data;
using MonParfumApp.Models;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MonParfumApp.Controllers
{
    [Authorize]
    public class PanierController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PanierController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Afficher le panier de l'utilisateur authentifié
        public async Task<IActionResult> Index()
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Récupérer l'identifiant de l'utilisateur


            // Chercher le panier de l'utilisateur, ou en créer un si nécessaire
            var panier = await _context.Paniers
                .Include(p => p.Items)
                    .ThenInclude(pi => pi.Parfum)
                .FirstOrDefaultAsync(p => p.ClientId == clientId);

            if (panier == null)
            {
                panier = new Panier
                {
                    ClientId = clientId,
                    Items = new List<PanierItem>()
                };
                _context.Paniers.Add(panier);
                await _context.SaveChangesAsync();
            }

            // Calculer le total dynamiquement
            ViewBag.TotalPanier = panier.Items.Sum(i => i.Quantite * i.Parfum.Prix);

            return View(panier); // Passer le panier à la vue
        }

        // Ajouter un article au panier
        [HttpPost]
        public async Task<IActionResult> AjouterAuPanier(int parfumId)
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(clientId))
            {
                return Unauthorized("Utilisateur non authentifié.");
            }

            var parfum = await _context.Parfums.FindAsync(parfumId);
            if (parfum == null)
            {
                return NotFound("Le parfum demandé n'existe pas.");
            }

            // Récupérer ou créer le panier de l'utilisateur
            var panier = await _context.Paniers
                .Include(p => p.Items)
                    .ThenInclude(pi => pi.Parfum)
                .FirstOrDefaultAsync(p => p.ClientId == clientId);

            if (panier == null)
            {
                panier = new Panier
                {
                    ClientId = clientId,
                    Items = new List<PanierItem>()
                };
                _context.Paniers.Add(panier);
                await _context.SaveChangesAsync();
            }

            // Vérifier si l'article est déjà dans le panier
            var item = panier.Items.FirstOrDefault(i => i.ParfumId == parfumId);
            if (item == null)
            {
                // Ajouter un nouvel item au panier
                panier.Items.Add(new PanierItem
                {
                    ParfumId = parfumId,
                    Quantite = 1, // Par défaut la quantité est 1
                    PanierId = panier.Id // Lier l'item au panier
                });
            }
            else
            {
                // Mettre à jour la quantité si le parfum est déjà dans le panier
                item.Quantite++;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Parfum ajouté au panier avec succès.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ModifierQuantite(int itemId, int nouvelleQuantite)
        {
            var item = _context.PanierItems.FirstOrDefault(i => i.Id == itemId);

            if (item != null && nouvelleQuantite > 0)
            {
                item.Quantite = nouvelleQuantite;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return BadRequest("Quantité non valide ou élément introuvable.");
        }















        // Supprimer un article du panier
        [HttpPost]
        public async Task<IActionResult> SupprimerDuPanier(int itemId)
        {
            var item = await _context.PanierItems.FindAsync(itemId);
            if (item != null)
            {
                _context.PanierItems.Remove(item);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Article supprimé du panier.";
            }

            return RedirectToAction("Index");
        }
    }
}