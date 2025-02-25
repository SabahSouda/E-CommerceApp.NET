namespace MonParfumApp.Models
{
    public class Categorie
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public ICollection<Parfum>? Parfums { get; set; } // Relation avec les Parfums
    }

}
