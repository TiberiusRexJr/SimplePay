using Cobra.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cobra.Api.ViewModels.Invoices;

namespace Cobra.Api.Controllers;

public class InvoicesController : Controller
{
    private readonly AppDbContext _db;
    public InvoicesController(AppDbContext db) => _db = db;

    // GET /Invoices
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
}

