using Microsoft.AspNetCore.Identity;

namespace MonParfumApp.Models
{
    public class Client : IdentityUser
    {
        // Ajout de champs spécifiques à votre application
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;

        // Liste des commandes associées au client
        public ICollection<Commande>? Commandes { get; set; }

        // Relation avec le panier
        public Panier? Panier { get; set; }
    }
}
