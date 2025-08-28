namespace Cobra.Api.ViewModels.Invoices;

public sealed record InvoiceRowVm(
    Guid Id,
    string Customer,
    decimal Total,
    DateTime? DueDate
);
