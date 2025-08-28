using System.ComponentModel.DataAnnotations;

namespace Cobra.Api.ViewModels.Payments;

public sealed class PaymentCreateVm
{
    public Guid InvoiceId { get; set; }
    public string Customer { get; set; } = "";
    public decimal Total { get; set; }
    public decimal Paid { get; set; }
    public decimal Balance { get; set; }

    [Range(0.01, 1_000_000)]
    public decimal Amount { get; set; }

    [StringLength(60)] public string? Method { get; set; }
    [StringLength(120)] public string? Reference { get; set; }
}
