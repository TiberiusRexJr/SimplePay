using Cobra.Api.DTO.Billing;
using Cobra.Api.Services;
using Cobra.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cobra.Api.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IBillingService _billing;
        public PaymentsController(IBillingService billing) => _billing = billing;

        // Named route for CreatedAtRoute from BillingController
        [HttpGet("{paymentId:guid}", Name = "GetPaymentById")]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentDto>> GetById(Guid paymentId, CancellationToken ct)
        {
            var payment = await _billing.GetPaymentByIdAsync(paymentId, ct);
            return payment is null ? NotFound() : Ok(payment);
        }

        // Convenience: list payments for an invoice
        [HttpGet("by-invoice/{invoiceId:guid}")]
        [ProducesResponseType(typeof(List<PaymentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PaymentDto>>> GetByInvoice(Guid invoiceId, CancellationToken ct)
        {
            var items = await _billing.GetPaymentsForInvoiceAsync(invoiceId, ct);
            return Ok(items);
        }
    }
}
