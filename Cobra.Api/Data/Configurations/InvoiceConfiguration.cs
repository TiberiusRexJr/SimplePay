using Cobra.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cobra.Api.Data.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> b)
        {
            b.ToTable("Invoices");

            // Key
            b.HasKey(i => i.Id);

            // Required FKs / fields
            b.Property(i => i.CustomerId).IsRequired();

            // Dates (entity uses DateTime; if you use DateOnly, see note below)
            b.Property(i => i.InvoiceDate).IsRequired();
            b.Property(i => i.DueDate).IsRequired();

            // Money fields
            b.Property(i => i.Subtotal).HasPrecision(18, 2);
            b.Property(i => i.Tax).HasPrecision(18, 2);
            b.Property(i => i.Total).HasPrecision(18, 2);
            b.Property(i => i.Paid).HasPrecision(18, 2);

            // Balance is calculated in code; don't persist
            b.Ignore(i => i.Balance);

            // Concurrency
            b.Property(i => i.RowVersion)
             .IsRowVersion()
             .IsConcurrencyToken();

            // Enum storage (string for readability; use int if you prefer)
            b.Property(i => i.Status)
             .HasConversion<string>()
             .HasMaxLength(20)
             .IsRequired();

            // Relationships
            b.HasMany(i => i.Lines)
             .WithOne()                              // no navigation back on the line entity
             .HasForeignKey(l => l.InvoiceId)
             .OnDelete(DeleteBehavior.Cascade);

            // Useful indexes
            b.HasIndex(i => new { i.CustomerId, i.Status });
            b.HasIndex(i => i.DueDate);
        }
    }
}
