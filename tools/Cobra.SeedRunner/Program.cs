using Cobra.Api.Data;
using Cobra.Api.Data.Seed;
using Microsoft.EntityFrameworkCore;
using System;

var factory = new AppDbContextFactory();
using var db = factory.CreateDbContext(Array.Empty<string>());

// Make sure schema is present (creates/updates DB)
await db.Database.MigrateAsync();

// Seed (your AppDbSeeder should be idempotent)
await AppDbSeeder.SeedAsync(db);

// Feedback so you know it worked
var customers = await db.Customers.CountAsync();
var invoices = await db.Invoices.CountAsync();
Console.WriteLine($"Seed complete. Customers={customers}, Invoices={invoices}");
