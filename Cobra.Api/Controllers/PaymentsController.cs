using Cobra.Api.Data;
using Cobra.Api.Domain;
using Cobra.Api.ViewModels.Payments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cobra.Api.Controllers;

public class PaymentsController : Controller
{
    private readonly AppDbContext _db;
    public PaymentsController(AppDbContext db) => _db = db;

    // GET /Payments (navbar "Make Payment" should link here)
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var rows = await (
            from i in _db.Invoices
            join p in _db.Payments on i.Id equals p.InvoiceId into gj
            let paid = gj.Sum(x => (decimal?)x.Amount) ?? 0m
            let balance = i.Total - paid
            where balance > 0m
            orderby i.DueDate
            select new InvoiceToPayVm(
                i.Id,
                i.Customer.Name,
                i.Total,
                paid,
                balance,
                i.DueDate
            )
        ).AsNoTracking().ToListAsync(ct);

        return View(rows);
    }

    // GET /Payments/Pay/{id}
    public async Task<IActionResult> Pay(Guid id, CancellationToken ct)
    {
        var vm = await (
            from i in _db.Invoices
            where i.Id == id
            join p in _db.Payments on i.Id equals p.InvoiceId into gj
            let paid = gj.Sum(x => (decimal?)x.Amount) ?? 0m
            select new PaymentCreateVm
            {
                InvoiceId = i.Id,
                Customer = i.Customer.Name,
                Total = i.Total,
                Paid = paid,
                Balance = i.Total - paid,
                Amount = i.Total - paid // default to remaining balance
            }
        ).FirstOrDefaultAsync(ct);

        return vm is null ? NotFound() : View(vm);
    }

    // POST /Payments/Pay
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(PaymentCreateVm vm, CancellationToken ct)
    {
        var invoice = await _db.Invoices.FirstOrDefaultAsync(i => i.Id == vm.InvoiceId, ct);
        if (invoice is null) return NotFound();

        var paid = await _db.Payments
            .Where(p => p.InvoiceId == vm.InvoiceId)
            .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;

        var balance = invoice.Total - paid;

        if (vm.Amount <= 0 || vm.Amount > balance)
        {
            ModelState.AddModelError(nameof(vm.Amount), "Amount exceeds outstanding balance.");
            vm.Total = invoice.Total; vm.Paid = paid; vm.Balance = balance;
            return View(vm);
        }

        _db.Payments.Add(new Payment
        {
            Id = Guid.NewGuid(),
            InvoiceId = vm.InvoiceId,
            Amount = vm.Amount,
            PaidAt = DateTime.UtcNow,
            Method = vm.Method,
            Reference = vm.Reference
        });

        await _db.SaveChangesAsync(ct);
        TempData["Message"] = "Payment recorded.";
        return RedirectToAction(nameof(Index));
    }
}
