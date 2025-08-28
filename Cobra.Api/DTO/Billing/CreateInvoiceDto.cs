using System.ComponentModel.DataAnnotations;

namespace Cobra.Api.DTO.Billing
{
    public record CreateInvoiceDto(
       [Required] Guid CustomerId,
       [Required] DateOnly InvoiceDate,
       [Required] DateOnly DueDate,
       [MinLength(1)] List<InvoiceLineDto> Lines
   );

}
