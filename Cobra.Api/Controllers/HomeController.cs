using Cobra.Api.Data;
using Cobra.Api.ViewModels.Payments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cobra.Api.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    public HomeController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var cutoffUtc = DateTime.UtcNow.AddHours(-1);

        var rows = await _db.Payments
            .AsNoTracking()
            .Where(p => p.PaidAt >= cutoffUtc)
            .OrderByDescending(p => p.PaidAt)
            .Select(p => new PaymentRowVm(
                p.Id,
                p.InvoiceId,
                p.Invoice.Customer.Name,
                p.Amount,
                p.PaidAt,
                p.Method,
                p.Reference
            ))
            .ToListAsync(ct);

        ViewBag.CutoffUtc = cutoffUtc;
        return View(rows);
    }
}