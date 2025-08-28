using Cobra.Api.Data.Configurations;
using Cobra.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Cobra.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Payment> Payments => Set<Payment>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Automatically pick up IEntityTypeConfiguration<T> in this assembly
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
