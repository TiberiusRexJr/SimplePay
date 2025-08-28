// App.Tests/Controllers/BillingControllerTests.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cobra.Api.Controllers;
using Cobra.Api.DTO.Invoices;
using Cobra.Api.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cobra.Api.Tests.Controllers;
public class BillingControllerTests
{
    // Group tests by action for clarity
    public class GetInvoicesForCustomer
    {
        [Fact]
        public async Task Found_returns_200_with_list()
        {
            var customerId = Guid.NewGuid();
            var expected = new List<InvoiceSummaryDto> { new InvoiceSummaryDto() };

            var logger = NullLogger<BillingController>.Instance;
            var billing = new Mock<IBillingService>();
            billing
                .Setup(b => b.GetInvoicesForCustomerAsync(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var controller = new BillingController(billing.Object, logger);

            ActionResult<List<InvoiceSummaryDto>> result =
                await controller.GetInvoicesForCustomer(customerId, CancellationToken.None);

            result.Result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)result.Result!;
            ok.Value.Should().BeAssignableTo<List<InvoiceSummaryDto>>();
            ((List<InvoiceSummaryDto>)ok.Value!).Count.Should().Be(1);
        }

        [Fact]
        public async Task Missing_returns_404()
        {
            var missingId = Guid.NewGuid();

            var logger = NullLogger<BillingController>.Instance;
            var billing = new Mock<IBillingService>();
            billing
                .Setup(b => b.GetInvoicesForCustomerAsync(missingId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new KeyNotFoundException("Customer not found"));

            var controller = new BillingController(billing.Object, logger);

            Func<Task> act = async () =>
            await controller.GetInvoicesForCustomer(missingId, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("*Customer not found*");
        }
    }

}
