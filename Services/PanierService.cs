using Microsoft.EntityFrameworkCore;
using MonParfumApp.Data;
using MonParfumApp.Models;

namespace MonParfumApp.Services
{
    public class PanierService
    {
        private readonly ApplicationDbContext _context;

        public PanierService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Méthode pour récupérer le panier par ClientId (de type string)
        public async Task<Panier?> GetPanierByClientIdAsync(string clientId)
        {
            return await _context.Paniers
                .Include(p => p.Items)
                .ThenInclude(i => i.Parfum)
                .FirstOrDefaultAsync(p => p.ClientId == clientId);
        }

        // Ajouter un parfum dans le panier
        public async Task AddItemToPanierAsync(string clientId, int parfumId, int quantite)
        {
            var panier = await GetPanierByClientIdAsync(clientId) ?? new Panier { ClientId = clientId, Items = new List<PanierItem>() };

            // Recherche de l'item existant dans le panier
            var item = panier.Items.FirstOrDefault(i => i.ParfumId == parfumId);
            if (item == null)
            {
                // Si l'item n'existe pas, l'ajouter
                panier.Items.Add(new PanierItem { ParfumId = parfumId, Quantite = quantite });
            }
            else
            {
                // Si l'item existe déjà, on met à jour la quantité
                item.Quantite += quantite;
            }

            // Si le panier n'a pas encore été ajouté à la base de données, on l'ajoute
            if (panier.Id == 0)
            {
                _context.Paniers.Add(panier);
            }

            await _context.SaveChangesAsync();
        }

        // Supprimer un parfum du panier
        public async Task RemoveItemFromPanierAsync(string clientId, int parfumId)
        {
            var panier = await GetPanierByClientIdAsync(clientId);
            if (panier != null)
            {
                var item = panier.Items.FirstOrDefault(i => i.ParfumId == parfumId);
                if (item != null)
                {
                    // Suppression de l'item du panier
                    panier.Items.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
