using Microsoft.EntityFrameworkCore;

namespace AeroBites.Data
{
    public class AeroBitesContext : DbContext
    {
        public AeroBitesContext(DbContextOptions<AeroBitesContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<AeroBites.Models.Account> Account { set; get; } = default!;
        public DbSet<AeroBites.Models.PaymentMethod> PaymentMethod { set; get; } = default!;
        public DbSet<AeroBites.Models.AccountLog> AccountLog { set; get; } = default!;
        public DbSet<AeroBites.Models.Address> Address { set; get; } = default!;
        public DbSet<AeroBites.Models.Restaurant> Restaurant { set; get; } = default!;
        public DbSet<AeroBites.Models.Category> Category { set; get; } = default!;
        public DbSet<AeroBites.Models.Item> Item { set; get; } = default!;
        public DbSet<AeroBites.Models.Cart> Cart { set; get; } = default!;
        public DbSet<AeroBites.Models.CartAddress> CartAddress { set; get; } = default!;
        public DbSet<AeroBites.Models.CartItem> CartItem { set; get; } = default!;
    }
}
