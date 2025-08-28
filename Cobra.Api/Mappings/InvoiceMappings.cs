using Cobra.Api.Domain;
using Cobra.Api.DTO.Billing;
using Cobra.Api.DTO.Invoices;

namespace Cobra.Api.Mappings
{
    public static class InvoiceMappings
    {
        public static InvoiceDto ToDto(this Invoice i) =>
            new(
                Id: i.Id,
                CustomerId: i.CustomerId,
                Subtotal: i.Subtotal,
                Tax: i.Tax,
                Total: i.Total,
                Paid: i.Paid,
                Balance: i.Total - i.Paid,           // don't read i.Balance inside database projections
                Status: i.Status.ToString(),
                RowVersion: i.RowVersion
            );

        public static InvoiceSummaryDto ToSummaryDto(this Invoice i, string customerName) =>
            new()
            {
                Id = i.Id,
                CustomerId = i.CustomerId,
                CustomerName = customerName,
                InvoiceDate = DateOnly.FromDateTime(i.InvoiceDate),
                DueDate = DateOnly.FromDateTime(i.DueDate),
                Total = i.Total,
                Balance = i.Total - i.Paid,
                Status = i.Status.ToString()
            };

        public static InvoiceLineDto ToLineDto(this InvoiceLine l) =>
          new InvoiceLineDto
          {
              Description = l.Description,
              Quantity = l.Quantity,
              UnitPrice = l.UnitPrice
          };


    }
}
