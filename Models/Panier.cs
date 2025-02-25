namespace MonParfumApp.Models
{
    public class Panier
    {
        public int Id { get; set; }
        public string ClientId { get; set; }

        public Client? Client { get; set; } // Relation avec Client
        public ICollection<PanierItem>? Items { get; set; } // Liste des articles du panier
    }

    public class PanierItem
    {
        public int Id { get; set; }
        public int ParfumId { get; set; }
        public Parfum? Parfum { get; set; } // Relation avec Parfum
        public int Quantite { get; set; }
        public int PanierId { get; set; }
        public Panier? Panier { get; set; } // Relation avec Panier
    }

}
