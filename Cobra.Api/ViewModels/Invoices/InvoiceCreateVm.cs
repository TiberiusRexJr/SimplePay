using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Cobra.Api.ViewModels.Invoices
{
    public class InvoiceCreateVm
    {
        [Required] public Guid CustomerId { get; set; }

        [Range(0.01, 1_000_000)]
        public decimal Total { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; } = DateTime.UtcNow.Date.AddDays(7);

        public List<SelectListItem> Customers { get; set; } = new();
    }
}
