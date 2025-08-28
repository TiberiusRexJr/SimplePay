using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cobra.Api.Domain;

namespace Cobra.Api.Data.Configurations
{
    public class InvoiceLineConfiguration : IEntityTypeConfiguration<InvoiceLine>
    {
        public void Configure(EntityTypeBuilder<InvoiceLine> b)
        {
            b.ToTable("InvoiceLines");

            // Primary key
            b.HasKey(l => l.Id);

            // Foreign key back to Invoice
            b.Property(l => l.InvoiceId)
             .IsRequired();

            b.HasIndex(l => l.InvoiceId);

            // Description
            b.Property(l => l.Description)
             .HasMaxLength(160)
             .IsRequired();

            // Quantity and UnitPrice
            b.Property(l => l.Quantity)
             .HasPrecision(18, 2)          // store as decimal(18,2)
             .IsRequired();

            b.Property(l => l.UnitPrice)
             .HasPrecision(18, 2)
             .IsRequired();

            // Optional: computed column for line total
            // b.Property<decimal>("LineTotal")
            //  .HasPrecision(18, 2)
            //  .HasComputedColumnSql("[Quantity] * [UnitPrice]");
        }
    }
}
