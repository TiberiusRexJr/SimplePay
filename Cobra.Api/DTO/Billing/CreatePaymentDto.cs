using System.ComponentModel.DataAnnotations;

namespace Cobra.Api.DTO.Billing;

public record CreatePaymentDto(
    [Required] Guid InvoiceId,
    [Range(0.01, 1_000_000)] decimal Amount,
    [Required] DateTime PaidAt,
    [StringLength(60)] string? Method,
    [StringLength(120)] string? Reference
);
