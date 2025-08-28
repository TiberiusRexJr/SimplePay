namespace Cobra.Api.DTO.Billing
{
    public record InvoiceDto(
      Guid Id,
      Guid CustomerId,
      decimal Subtotal,
      decimal Tax,
      decimal Total,
      decimal Paid,
      decimal Balance,
      string Status,
      byte[]? RowVersion
  );
}
