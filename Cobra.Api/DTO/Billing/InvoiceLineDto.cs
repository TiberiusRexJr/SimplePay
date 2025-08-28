using System.ComponentModel.DataAnnotations;

namespace Cobra.Api.DTO.Billing
{
    public record InvoiceLineDto
    {
        [Required]
        [StringLength(160, MinimumLength = 2)]
        public string Description { get; init; } = string.Empty;

        [Range(0.01, 1_000_000)]
        public decimal Quantity { get; init; }

        [Range(0.00, 1_000_000)]
        public decimal UnitPrice { get; init; }
    }
}
