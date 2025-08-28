using Cobra.Api.Domain;

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
    }
}
