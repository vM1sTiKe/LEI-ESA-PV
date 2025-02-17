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
        public DbSet<AeroBites.Models.Restaurant> Restaurant { set; get; } = default!;
        public DbSet<AeroBites.Models.Category> Category { set; get; } = default!;
        public DbSet<AeroBites.Models.Item> Item { set; get; } = default!;
        public DbSet<AeroBites.Models.DropPoint> DropPoint { set; get; } = default!;
        public DbSet<AeroBites.Models.DropPointFavourite> DropPointFavourite { set; get; } = default!;
        public DbSet<AeroBites.Models.Order> Order { set; get; } = default!;
        public DbSet<AeroBites.Models.OrderItem> OrderItem { set; get; } = default!;
        public DbSet<AeroBites.Models.Payment> Payment { set; get; } = default!;
        public DbSet<AeroBites.Models.Address> Address { set; get; } = default!;


    }
}
