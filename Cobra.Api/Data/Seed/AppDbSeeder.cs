using Cobra.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Cobra.Api.Data.Seed
{
    public class AppDbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            if (!db.Customers.Any())
            {
                var c1 = new Customer { Name = "Jane Carter", Email = "jane@example.com" };
                var c2 = new Customer { Name = "Mark Diaz", Email = "mark@example.com" };

                db.Customers.AddRange(c1, c2);
                await db.SaveChangesAsync();

                db.Invoices.AddRange(
                    new Invoice { CustomerId = c1.Id, Total = 120.50m, DueDate = DateTime.UtcNow.AddDays(7) },
                    new Invoice { CustomerId = c1.Id, Total = 89.99m, DueDate = DateTime.UtcNow.AddDays(14) },
                    new Invoice { CustomerId = c2.Id, Total = 340.00m, DueDate = DateTime.UtcNow.AddDays(10) }
                );
                await db.SaveChangesAsync();
            }
        }

        public static async Task EnsureSeeded(AppDbContext db)
        {
            if (await db.Customers.AnyAsync()) return; // idempotent

            var c1 = new Customer { Id = Guid.NewGuid(), Name = "Jane Carter", Email = "jane@example.com" };
            var c2 = new Customer { Id = Guid.NewGuid(), Name = "Mark Diaz", Email = "mark@example.com" };

            db.Customers.AddRange(c1, c2);
            await db.SaveChangesAsync();

            var i1 = new Invoice { Id = Guid.NewGuid(), CustomerId = c1.Id, Total = 120.50m, DueDate = DateTime.UtcNow.AddDays(7) };
            var i2 = new Invoice { Id = Guid.NewGuid(), CustomerId = c1.Id, Total = 89.99m, DueDate = DateTime.UtcNow.AddDays(14) };
            var i3 = new Invoice { Id = Guid.NewGuid(), CustomerId = c2.Id, Total = 340.00m, DueDate = DateTime.UtcNow.AddDays(10) };
            db.Invoices.AddRange(i1, i2, i3);
            await db.SaveChangesAsync();

            db.Payments.AddRange(
                new Payment { Id = Guid.NewGuid(), InvoiceId = i1.Id, Amount = 50m, PaidAt = DateTime.UtcNow, Method = "Card", Reference = "TEST001" },
                new Payment { Id = Guid.NewGuid(), InvoiceId = i3.Id, Amount = 100m, PaidAt = DateTime.UtcNow, Method = "ACH", Reference = "TEST002" }
            );
            await db.SaveChangesAsync();
        }
    }
}
