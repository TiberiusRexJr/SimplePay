namespace Cobra.Api.ViewModels.Payments;

public sealed record PaymentRowVm(
    Guid Id,
    Guid InvoiceId,
    decimal Amount,
    DateTime PaidAt,
    string? Method,
    string? Reference
);
