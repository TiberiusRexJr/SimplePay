namespace Cobra.Api.DTO.Billing;

public record PaymentDto(
    Guid Id,
    Guid InvoiceId,
    decimal Amount,
    DateTime PaidAt,
    string? Method,
    string? Reference,
    byte[]? RowVersion
);
