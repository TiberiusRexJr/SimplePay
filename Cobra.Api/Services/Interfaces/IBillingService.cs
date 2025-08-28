using Cobra.Api.Domain;
using Cobra.Api.DTO.Billing;
using Cobra.Api.DTO.Invoices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cobra.Api.Services.Interfaces
{
    public interface IBillingService
    {
        /// <summary>
        /// Creates a new invoice for a customer, with optional line items.
        /// </summary>
        /// <param name="dto">Invoice creation payload</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Created invoice with totals/status</returns>
        Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto, CancellationToken ct);

        /// <summary>
        /// Records a payment against an invoice and updates invoice status/balance.
        /// </summary>
        /// <param name="dto">Payment creation payload</param>
        /// <param name="idempotencyKey">Optional client-provided key to prevent double-charges</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Created payment</returns>
        Task<PaymentDto> RecordPaymentAsync(CreatePaymentDto dto, string? idempotencyKey, CancellationToken ct);
        Task<List<InvoiceSummaryDto>> GetInvoicesForCustomerAsync(Guid customerId, CancellationToken ct);

        /// <summary>
        /// Gets a unified view of an invoice, its customer, and related payments.
        /// </summary>
        /// <param name="invoiceId">Invoice identifier</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Snapshot with invoice, customer summary, and payments</returns>
        Task<InvoiceSnapshotDto> GetInvoiceSnapshotAsync(Guid invoiceId, CancellationToken ct);

        // (new, optional) reads for payments:
        Task<PaymentDto?> GetPaymentByIdAsync(Guid paymentId, CancellationToken ct);
        Task<List<PaymentDto>> GetPaymentsForInvoiceAsync(Guid invoiceId, CancellationToken ct);

    }
}
