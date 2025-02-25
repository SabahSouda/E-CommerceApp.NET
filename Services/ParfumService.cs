using Microsoft.EntityFrameworkCore;
using MonParfumApp.Data;
using MonParfumApp.Models;

namespace MonParfumApp.Services
{
    public class ParfumService
    {
        private readonly ApplicationDbContext _context;

        public ParfumService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Récupérer tous les parfums avec leur catégorie

        public async Task<List<Parfum>> GetAllParfumsAsync()
        {
            return await _context.Parfums
                                 .Include(p => p.Categorie)
                                 .ToListAsync();
        }

        // Récupérer un parfum par son ID avec sa catégorie
        public async Task<Parfum?> GetParfumByIdAsync(int id)
        {
            return await _context.Parfums
                                 .Include(p => p.Categorie)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Ajouter un parfum
        public async Task AddParfumAsync(Parfum parfum)
        {
            var categorie = await _context.Categories.FirstOrDefaultAsync(c => c.Id == parfum.CategorieId);
            if (categorie != null)
            {
                parfum.Categorie = categorie; // Lier la catégorie au parfum
                _context.Parfums.Add(parfum);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("La catégorie spécifiée est invalide.");
            }
        }





        // Mettre à jour un parfum
        public async Task UpdateParfumAsync(Parfum parfum)
        {
            if (parfum == null)
                throw new ArgumentNullException(nameof(parfum), "Le parfum ne peut pas être null.");

            // Vérifier si le parfum existe
            var existingParfum = await GetParfumByIdAsync(parfum.Id);
            if (existingParfum == null)
                throw new KeyNotFoundException("Le parfum spécifié n'existe pas.");

            // Vérifier si la catégorie existe
            var categorie = await _context.Categories.FirstOrDefaultAsync(c => c.Id == parfum.CategorieId);
            if (categorie == null)
                throw new ArgumentException("La catégorie spécifiée est invalide.");

            // Mettre à jour les propriétés
            existingParfum.Nom = parfum.Nom;
            existingParfum.Description = parfum.Description;
            existingParfum.Prix = parfum.Prix;
            existingParfum.CategorieId = parfum.CategorieId;
            existingParfum.Categorie = categorie;

            _context.Parfums.Update(existingParfum);
            await _context.SaveChangesAsync();
        }

        // Supprimer un parfum par son ID
        public async Task DeleteParfumAsync(int id)
        {
            var parfum = await GetParfumByIdAsync(id);
            if (parfum == null)
                throw new KeyNotFoundException("Le parfum spécifié n'existe pas.");

            _context.Parfums.Remove(parfum);
            await _context.SaveChangesAsync();
        }
    }
}
