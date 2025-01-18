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
    }
}
