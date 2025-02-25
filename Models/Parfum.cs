using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonParfumApp.Models
{
    public class Parfum
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du parfum est obligatoire.")]
        public required string Nom { get; set; }

        [Required(ErrorMessage = "La description du parfum est obligatoire.")]
        public required string Description { get; set; }

        [Range(0, 1000, ErrorMessage = "Le prix doit être un nombre valide entre 0 et 1000.")]
        public decimal Prix { get; set; }
        public string? ImagePath { get; set; }
        // Champ utilisé pour le téléchargement de fichier
        [NotMapped]
        [Required(ErrorMessage = "Le champ ImageFile est obligatoire.")]
        public IFormFile? ImageFile { get; set; }
        [Required(ErrorMessage = "La catégorie est obligatoire.")]
        public int CategorieId { get; set; } // C'est ici que vous vérifiez que la catégorie a été sélectionnée

        public virtual required Categorie? Categorie { get; set; }

       

    }

}


