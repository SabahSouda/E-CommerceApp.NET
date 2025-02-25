using Microsoft.EntityFrameworkCore;
using MonParfumApp.Data;
using MonParfumApp.Models;
using System.Linq;

namespace MonParfumApp.Services
{
    public class CommandeService
    {
        private readonly ApplicationDbContext _context;

        public CommandeService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Méthode pour récupérer les commandes d'un client par clientId de type string
        public async Task<List<Commande>> GetCommandesByClientIdAsync(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("Le clientId ne peut pas être null ou vide.", nameof(clientId));
            }

            return await _context.Commandes
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Parfum)
                .Where(c => c.ClientId == clientId) // Utilisation du clientId de type string
                .OrderByDescending(c => c.DateCommande) // Tri par date de commande décroissante
                .ToListAsync();
        }

        // Méthode pour créer une commande avec panierItems pour un clientId donné
        public async Task CreateCommandeAsync(string clientId, List<PanierItem> panierItems)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("Le clientId ne peut pas être null ou vide.", nameof(clientId));
            }

            if (panierItems == null || !panierItems.Any())
            {
                throw new ArgumentException("Le panier ne peut pas être vide.", nameof(panierItems));
            }

            var commande = new Commande
            {
                ClientId = clientId, // Utilisation du clientId de type string
                DateCommande = DateTime.Now,
                Total = panierItems.Sum(i => i.Parfum.Prix * i.Quantite), // Calcul du total de la commande
                Items = panierItems.Select(i => new CommandeItem
                {
                    ParfumId = i.ParfumId,
                    Quantite = i.Quantite,
                    PrixUnitaire = i.Parfum.Prix
                }).ToList()
            };

            // Ajouter la commande au contexte
            _context.Commandes.Add(commande);

            // Sauvegarder les modifications dans la base de données
            await _context.SaveChangesAsync();
        }

        // Nouvelle méthode pour récupérer les quantités totales de parfums commandés
        public async Task<Dictionary<string, int>> GetParfumQuantitiesAsync()
        {
            var result = await _context.CommandeItems
                .Include(ci => ci.Parfum)
                .GroupBy(ci => ci.Parfum.Nom)
                .Select(group => new
                {
                    ParfumName = group.Key,
                    TotalQuantity = group.Sum(ci => ci.Quantite)
                })
                .ToListAsync();

            return result.ToDictionary(r => r.ParfumName, r => r.TotalQuantity);
        }
    }
}
