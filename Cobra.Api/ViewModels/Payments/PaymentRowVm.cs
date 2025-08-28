namespace Cobra.Api.ViewModels.Payments;

public sealed record PaymentRowVm(
    Guid Id,
    Guid InvoiceId,
    String CustomerName,
    decimal Amount,
    DateTime PaidAt,
    string? Method,
    string? Reference
);
