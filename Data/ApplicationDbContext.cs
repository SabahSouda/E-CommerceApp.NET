using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MonParfumApp.Models;

namespace MonParfumApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<Client> // Utilise Client comme utilisateur personnalisé
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Parfum> Parfums { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Panier> Paniers { get; set; }
        public DbSet<PanierItem> PanierItems { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<CommandeItem> CommandeItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Parfum>()
               .HasOne(p => p.Categorie) // Relation 1-N
               .WithMany(c => c.Parfums)
               .HasForeignKey(p => p.CategorieId)
               .OnDelete(DeleteBehavior.Cascade); // Suppression en cascade optionnelle


            modelBuilder.Entity<Panier>()
                 .HasOne(p => p.Client)
                 .WithOne(c => c.Panier)
                 .HasForeignKey<Panier>(p => p.ClientId)
                 .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<PanierItem>()
                .HasOne(pi => pi.Parfum)
                .WithMany()
                .HasForeignKey(pi => pi.ParfumId);

            modelBuilder.Entity<Commande>()
            .HasOne(c => c.Client) // Relation avec Client
            .WithMany(cli => cli.Commandes) // Un Client peut avoir plusieurs Commandes
            .HasForeignKey(c => c.ClientId) // La clé étrangère est ClientId
            .OnDelete(DeleteBehavior.Cascade); // Si Client est supprimé, toutes ses commandes le seront aussi


            modelBuilder.Entity<CommandeItem>()
                    .HasOne(ci => ci.Parfum)
                    .WithMany()
                    .HasForeignKey(ci => ci.ParfumId);
        }
    }
}
