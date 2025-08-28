namespace Cobra.Api.DTO.Invoices
{
    public record InvoiceSummaryDto
    {
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public string CustomerName { get; init; } = string.Empty; // helpful for quick list views
        public DateOnly InvoiceDate { get; init; }
        public DateOnly DueDate { get; init; }
        public decimal Total { get; init; }
        public decimal Balance { get; init; }
        public string Status { get; init; } = string.Empty;
    }
}
