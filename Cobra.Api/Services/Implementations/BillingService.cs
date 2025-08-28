using Cobra.Api.Data;
using Cobra.Api.Domain;
using Cobra.Api.DTO.Billing;
using Cobra.Api.DTO.Customers;
using Cobra.Api.DTO.Invoices;
using Cobra.Api.Mappings;
using Cobra.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Cobra.Api.Errors;


namespace Cobra.Api.Services.Implementations
{
    public class BillingService : IBillingService
    {
        private readonly AppDbContext _db;

        public BillingService(AppDbContext db) => _db = db;

        /// <summary>
        /// Create an invoice for a customer (with optional lines) atomically.
        /// </summary>
        public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto, CancellationToken ct)
        {
            if (dto.Lines is null || dto.Lines.Count == 0)
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    ["Lines"] = new[] { "At least one line is required." }
                }, "Invalid invoice input.");

            var customerExists = await _db.Customers
                .AsNoTracking()
                .AnyAsync(c => c.Id == dto.CustomerId, ct);

            if (!customerExists)
                throw new NotFoundException("Customer not found.");

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                CustomerId = dto.CustomerId,
                InvoiceDate = dto.InvoiceDate.ToDateTime(TimeOnly.MinValue),
                DueDate = dto.DueDate.ToDateTime(TimeOnly.MinValue),
                Status = InvoiceStatus.Draft
            };

            if (dto.Lines is not null)
            {
                foreach (var l in dto.Lines)
                {
                    if (l.Quantity <= 0m)
                        throw new ValidationException(new Dictionary<string, string[]>
                        {
                            ["Lines.Quantity"] = new[] { "Quantity must be greater than zero." }
                        });

                    if (l.UnitPrice < 0m)
                        throw new ValidationException(new Dictionary<string, string[]>
                        {
                            ["Lines.UnitPrice"] = new[] { "Unit price cannot be negative." }
                        });

                    invoice.AddLine(l.Description, l.Quantity, l.UnitPrice);
                }
            }

            invoice.RecalculateTotals();
            invoice.Status = InvoiceStatus.Open;

            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync(ct);

            return invoice.ToDto();
        }

        /// <summary>
        /// Record a payment and atomically update the invoice's Paid/Status values.
        /// </summary>
        public async Task<PaymentDto> RecordPaymentAsync(CreatePaymentDto dto, string? idempotencyKey, CancellationToken ct)
        {
            if (dto.Amount <= 0m)
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    ["Amount"] = new[] { "Payment amount must be greater than zero." }
                });

            // NOTE: idempotencyKey is accepted for future use (table not required here)
            _ = idempotencyKey; // silence unused warning if you haven't added IdempotencyKey storage yet

            using var tx = await _db.Database.BeginTransactionAsync(ct);

            var invoice = await _db.Invoices
                .FirstOrDefaultAsync(i => i.Id == dto.InvoiceId, ct)
                ?? throw new NotFoundException("Invoice not found.");

            if (invoice.Status is InvoiceStatus.Canceled or InvoiceStatus.Void)
                throw new BusinessRuleException("Invoice is not payable.");

            if (dto.Amount > invoice.Total - invoice.Paid)
                throw new BusinessRuleException("Payment exceeds outstanding balance.");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                InvoiceId = dto.InvoiceId,
                Amount = dto.Amount,
                PaidAt = dto.PaidAt,
                Method = dto.Method,
                Reference = dto.Reference
            };

            _db.Payments.Add(payment);

            // Domain behavior updates Paid and Status; throws if invalid (e.g., overpayment if you enforce)
            invoice.ApplyPayment(dto.Amount, dto.PaidAt);
            _db.Invoices.Update(invoice);

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return payment.ToDto();
        }
        /// <summary>
        /// Convenience query: list summaries of a customer's invoices.
        /// </summary>
        public async Task<List<InvoiceSummaryDto>> GetInvoicesForCustomerAsync(Guid customerId, CancellationToken ct)
        {
            // Join to customers to include CustomerName without requiring a nav property
            var items = await _db.Invoices
                .Where(i => i.CustomerId == customerId && i.Balance > 0)
                .Join(_db.Customers,
                      i => i.CustomerId,
                      c => c.Id,
                      (i, c) => new InvoiceSummaryDto
                      {
                          Id = i.Id,
                          CustomerId = i.CustomerId,
                          CustomerName = c.Name,
                          InvoiceDate = DateOnly.FromDateTime(i.InvoiceDate),
                          DueDate = DateOnly.FromDateTime(i.DueDate),
                          Total = i.Total,
                          Balance = i.Total - i.Paid,
                          Status = i.Status.ToString()
                      })
                .OrderByDescending(x => x.InvoiceDate)
                .ToListAsync(ct);

            return items;
        }

        /// <summary>
        /// Get a composite view (invoice + customer summary + payments).
        /// </summary>
        /// <summary>
        /// Get a composite view (invoice + customer summary + payments).
        /// </summary>
        public async Task<InvoiceSnapshotDto> GetInvoiceSnapshotAsync(Guid invoiceId, CancellationToken ct)
        {
            // Load invoice & customer (either via join or nav if you added it)
            var invoiceJoin = await _db.Invoices
                .Where(i => i.Id == invoiceId)
                .Join(_db.Customers,
                      i => i.CustomerId,
                      c => c.Id,
                      (i, c) => new
                      {
                          Invoice = i,
                          Customer = new CustomerSummaryDto
                          {
                              Id = c.Id,
                              Name = c.Name,
                              Email = c.Email
                          }
                      })
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (invoiceJoin is null)
                throw new KeyNotFoundException("Invoice not found.");

            var payments = await _db.Payments
                .AsNoTracking()
                .Where(p => p.InvoiceId == invoiceId)
                .OrderBy(p => p.PaidAt)
                .Select(p => p.ToDto())
                .ToListAsync(ct);

            return new InvoiceSnapshotDto(
                Invoice: invoiceJoin.Invoice.ToDto(),
                Customer: invoiceJoin.Customer,
                Payments: payments
            );
        }
        public async Task<PaymentDto?> GetPaymentByIdAsync(Guid paymentId, CancellationToken ct)
        {
            var p = await _db.Payments.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == paymentId, ct);
            return p?.ToDto();
        }

        public async Task<List<PaymentDto>> GetPaymentsForInvoiceAsync(Guid invoiceId, CancellationToken ct)
        {
            return await _db.Payments.AsNoTracking()
                .Where(p => p.InvoiceId == invoiceId)
                .OrderBy(p => p.PaidAt)
                .Select(p => p.ToDto())
                .ToListAsync(ct);
        }


    }
}
