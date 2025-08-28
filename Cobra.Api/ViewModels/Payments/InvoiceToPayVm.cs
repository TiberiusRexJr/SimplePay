namespace Cobra.Api.ViewModels.Payments
{
    public sealed record InvoiceToPayVm(
    Guid Id,
    string Customer,
    decimal Total,
    decimal Paid,
    decimal Balance,
    DateTime? DueDate
);
}
