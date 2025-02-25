using Microsoft.EntityFrameworkCore;
using MonParfumApp.Data;
using MonParfumApp.Models;

namespace MonParfumApp.Services
{
    public class CategorieService
    {
        private readonly ApplicationDbContext _context;

        public CategorieService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Categorie>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Categorie?> GetCategorieByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddCategorieAsync(Categorie categorie)
        {
            _context.Categories.Add(categorie);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategorieAsync(Categorie categorie)
        {
            _context.Categories.Update(categorie);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategorieAsync(int id)
        {
            // Récupère la catégorie par son identifiant
            var categorie = await GetCategorieByIdAsync(id);

            // Si la catégorie existe, on la supprime
            if (categorie != null)
            {
                _context.Categories.Remove(categorie); // Marque la catégorie pour suppression
                await _context.SaveChangesAsync();     // Sauvegarde les changements dans la base de données
            }
        }


    }

}
