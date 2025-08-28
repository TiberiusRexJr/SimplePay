using Cobra.Api.DTO.Billing;
using Cobra.Api.DTO.Customers;
namespace Cobra.Api.DTO.Billing;
public record InvoiceSnapshotDto(
    InvoiceDto Invoice,
    CustomerSummaryDto Customer,
    IReadOnlyList<PaymentDto> Payments
);
