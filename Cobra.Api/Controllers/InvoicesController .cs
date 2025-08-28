using Cobra.Api.Data;
using Cobra.Api.Domain;
using Cobra.Api.ViewModels.Invoices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cobra.Api.Controllers;

public class InvoicesController : Controller
{
    private readonly AppDbContext _db;
    public InvoicesController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var rows = await _db.Invoices
            .OrderByDescending(i => i.DueDate)
            .Select(i => new InvoiceRowVm(
        i.Id,
        i.Customer.Name,
        i.Total,
        i.DueDate
    )).ToListAsync();


        return View(rows);
    }

    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new InvoiceCreateVm
        {
            Customers = await GetCustomerOptions(ct)
        };
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InvoiceCreateVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            vm.Customers = await GetCustomerOptions(ct);
            return View(vm);
        }

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            CustomerId = vm.CustomerId,
            Total = vm.Total,
            DueDate = vm.DueDate?.Date ?? DateTime.UtcNow.Date.AddDays(7)
        };

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync(ct);

        TempData["Message"] = "Invoice created.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> GetCustomerOptions(CancellationToken ct) =>
        await _db.Customers.AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToListAsync(ct);
}

