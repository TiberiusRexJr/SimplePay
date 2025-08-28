using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cobra.Api.Domain
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // FK → Invoice (your Invoice.CustomerId is Guid; keep this Guid too)
        public Guid InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = default!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

        [StringLength(60)]
        public string? Method { get; set; }   // e.g., "Card", "ACH", "Cash"

        [StringLength(120)]
        public string? Reference { get; set; } // external ref/txn id

        [Timestamp] public byte[]? RowVersion { get; set; } // optimistic concurrency
    }
}
