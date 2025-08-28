using Cobra.Api.DTO.Billing;
using Cobra.Api.DTO.Invoices;
using Cobra.Api.Services;
using Cobra.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cobra.Api.Errors;


namespace Cobra.Api.Controllers
{
    [ApiController]
    [Route("api/billing")]
    public class BillingController : ControllerBase
    {
        private readonly IBillingService _billing;
        private readonly ILogger<BillingController> _logger;

        public BillingController(IBillingService billing, ILogger<BillingController> logger)
        {
            _billing = billing;
            _logger = logger;
        }

        /// <summary>
        /// Create a new invoice for a customer.
        /// </summary>
        [HttpPost("customers/{customerId:guid}/invoices")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoiceDto>> CreateInvoiceForCustomer(
            Guid customerId,
            [FromBody] CreateInvoiceDto dto,
            CancellationToken ct)
        {
            if (dto.CustomerId != customerId)
                return BadRequest("CustomerId in route and body must match.");

            try
            {
                InvoiceDto invoice = await _billing.CreateInvoiceAsync(dto, ct);

                return CreatedAtRoute(
                    routeName: "GetInvoiceById",
                    routeValues: new { invoiceId = invoice.Id },
                    value: invoice
                );
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed creating invoice for {CustomerId}", customerId);
                return BadRequest(new ValidationProblemDetails(ex.Errors!)
                {
                    Title = ex.Message ?? "Validation failed."
                });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Customer {CustomerId} not found", customerId);
                return NotFound(new ProblemDetails { Title = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                _logger.LogWarning(ex, "Business rule violation creating invoice for {CustomerId}", customerId);
                return Conflict(new ProblemDetails { Title = ex.Message });
            }
        }

        /// <summary>
        /// Get all invoice summaries for a given customer.
        /// </summary>
        [HttpGet("customers/{customerId:guid}/invoices")]
        [ProducesResponseType(typeof(List<InvoiceSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<InvoiceSummaryDto>>> GetInvoicesForCustomer(
            Guid customerId,
            CancellationToken ct)
        {
            List<InvoiceSummaryDto> list = await _billing.GetInvoicesForCustomerAsync(customerId, ct);
            return Ok(list);
        }

        /// <summary>
        /// Get a full snapshot of an invoice (invoice + customer summary + payments).
        /// </summary>
        [HttpGet("invoices/{invoiceId:guid}/snapshot", Name = "GetInvoiceById")]
        [ProducesResponseType(typeof(InvoiceSnapshotDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoiceSnapshotDto>> GetInvoiceSnapshot(
            Guid invoiceId,
            CancellationToken ct)
        {
            try
            {
                InvoiceSnapshotDto snapshot = await _billing.GetInvoiceSnapshotAsync(invoiceId, ct);
                return Ok(snapshot);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Invoice {InvoiceId} not found", invoiceId);
                return NotFound(new ProblemDetails { Title = ex.Message });
            }
        }

        /// <summary>
        /// Record a payment for an invoice.
        /// </summary>
        [HttpPost("invoices/{invoiceId:guid}/payments")]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentDto>> RecordPayment(
            Guid invoiceId,
            [FromBody] CreatePaymentDto dto,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
            CancellationToken ct)
        {
            if (dto.InvoiceId != invoiceId)
                return BadRequest("InvoiceId in route and body must match.");

            try
            {
                PaymentDto payment = await _billing.RecordPaymentAsync(dto, idempotencyKey, ct);

                return CreatedAtRoute(
                    routeName: "GetPaymentById",
                    routeValues: new { paymentId = payment.Id },
                    value: payment
                );
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed recording payment for {InvoiceId}", invoiceId);
                return BadRequest(new ValidationProblemDetails(ex.Errors!)
                {
                    Title = ex.Message ?? "Validation failed."
                });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Invoice {InvoiceId} not found", invoiceId);
                return NotFound(new ProblemDetails { Title = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                _logger.LogWarning(ex, "Payment rule violation for invoice {InvoiceId}", invoiceId);
                return Conflict(new ProblemDetails { Title = ex.Message });
            }
        }

    }
}
