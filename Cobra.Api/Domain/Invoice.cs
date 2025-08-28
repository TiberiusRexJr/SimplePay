using Cobra.Api.Domain;
using Cobra.Api.DTO.Billing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cobra.Api.Domain
{
    public enum InvoiceStatus { Draft, Open, Partial, Paid, Canceled, Void }
    public class Invoice
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; private set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax { get; private set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Paid { get; private set; }

        [NotMapped] public decimal Balance => Total - Paid;

        public InvoiceStatus Status { get; set; }

        public List<InvoiceLine> Lines { get; } = new();

        [Timestamp] public byte[]? RowVersion { get; set; }

        public static Invoice Create(CreateInvoiceDto dto)
        {
            var inv = new Invoice
            {
                Id = Guid.NewGuid(),
                CustomerId = dto.CustomerId,
                InvoiceDate = dto.InvoiceDate.ToDateTime(TimeOnly.MinValue),
                DueDate = dto.DueDate.ToDateTime(TimeOnly.MinValue),
                Status = InvoiceStatus.Draft
            };

            foreach (var l in dto.Lines)
                inv.AddLine(l.Description, l.Quantity, l.UnitPrice);

            inv.RecalculateTotals();
            inv.Status = InvoiceStatus.Open;
            return inv;
        }

        public void AddLine(string description, decimal qty, decimal unitPrice)
        {
            if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
            if (unitPrice < 0) throw new ArgumentOutOfRangeException(nameof(unitPrice));
            Lines.Add(new InvoiceLine { Description = description, Quantity = qty, UnitPrice = unitPrice });
        }

        public void RecalculateTotals(decimal taxRate = 0m)
        {
            Subtotal = Lines.Sum(x => x.Quantity * x.UnitPrice);
            Tax = Math.Round(Subtotal * taxRate, 2, MidpointRounding.AwayFromZero);
            Total = Subtotal + Tax;
            // Paid unchanged here; Balance is computed
            UpdateStatusFromBalance();
        }

        public void ApplyPayment(decimal amount, DateTime paidAt)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (Status is InvoiceStatus.Canceled or InvoiceStatus.Void)
                throw new InvalidOperationException("Invoice not payable.");

            Paid += amount;
            UpdateStatusFromBalance();
        }

        private void UpdateStatusFromBalance()
        {
            Status = Balance switch
            {
                < 0m => throw new InvalidOperationException("Overpayment not allowed."),
                0m => InvoiceStatus.Paid,
                _ when Paid == 0m => InvoiceStatus.Open,
                _ => InvoiceStatus.Partial
            };
        }
    }

    public class InvoiceLine
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid InvoiceId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public string Description { get; set; } = string.Empty;
    }

}
