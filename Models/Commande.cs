namespace MonParfumApp.Models
{
    public class Commande
    {
        public int Id { get; set; }
        public DateTime DateCommande { get; set; }
        public decimal Total { get; set; }
        public string ClientId { get; set; }
        public Client? Client { get; set; } // Relation avec Client
        public ICollection<CommandeItem>? Items { get; set; } // Liste des articles de la commande
    }

    public class CommandeItem
    {
        public int Id { get; set; }
        public int ParfumId { get; set; }
        public Parfum? Parfum { get; set; } // Relation avec Parfum
        public int Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }
        public int CommandeId { get; set; }
        public Commande? Commande { get; set; }
    }

}
