using Cobra.Api.Domain;
using Cobra.Api.DTO.Billing;

namespace Cobra.Api.DTO.Customers
{
    public static class PaymentMappings
    {
        public static PaymentDto ToDto(this Payment p) =>
               new PaymentDto(
                   Id: p.Id,
                   InvoiceId: p.InvoiceId,
                   Amount: p.Amount,
                   PaidAt: p.PaidAt,
                   Method: p.Method,
                   Reference: p.Reference,
                   RowVersion: p.RowVersion
               );
    }
}
