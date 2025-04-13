using AeroBites.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AeroBites.Data
{
    public class AeroBitesContext : DbContext
    {
        public AeroBitesContext(DbContextOptions<AeroBitesContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Não remover pedidos ao apagar restaurantes
			modelBuilder.Entity<Cart>().HasOne(e => e.Restaurant).WithMany(u => u.Orders).OnDelete(DeleteBehavior.SetNull);
        }

        public DbSet<AeroBites.Models.Account> Account => Set<AeroBites.Models.Account>();
        public DbSet<AeroBites.Models.PaymentMethod> PaymentMethod => Set<AeroBites.Models.PaymentMethod>();
        public DbSet<AeroBites.Models.AccountLog> AccountLog => Set<AeroBites.Models.AccountLog>();
        public DbSet<AeroBites.Models.Address> Address => Set<AeroBites.Models.Address>();
        public DbSet<AeroBites.Models.Restaurant> Restaurant => Set<AeroBites.Models.Restaurant>();
        public DbSet<AeroBites.Models.Category> Category => Set<AeroBites.Models.Category>();
        public DbSet<AeroBites.Models.Item> Item => Set<AeroBites.Models.Item>();
        public DbSet<AeroBites.Models.Cart> Cart => Set<AeroBites.Models.Cart>();
        public DbSet<AeroBites.Models.CartAddress> CartAddress => Set<AeroBites.Models.CartAddress>();
        public DbSet<AeroBites.Models.CartItem> CartItem => Set<AeroBites.Models.CartItem>();
    }
}
