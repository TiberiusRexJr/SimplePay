using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cobra.Api.Domain;

namespace Cobra.Api.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> b)
        {
            b.ToTable("Payments");
            b.HasKey(p => p.Id);

            b.Property(p => p.InvoiceId).IsRequired();
            b.Property(p => p.Amount).HasPrecision(18, 2).IsRequired();
            b.Property(p => p.PaidAt).IsRequired();

            b.Property(p => p.Method).HasMaxLength(60);
            b.Property(p => p.Reference).HasMaxLength(120);

            b.Property(p => p.RowVersion)
             .IsRowVersion()
             .IsConcurrencyToken();

            b.HasIndex(p => p.InvoiceId);

        }
    }
}
