using Cobra.Api.Data;
using Cobra.Api.Domain;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Cobra.Api.Tests.Data
{
    public class SchemaTests
    {
        //dotnet ef migrations add AddCustomersPayments --project Cobra.Api --startup-project Cobra.Api --context AppDbContext

        [Fact]
        public void Model_and_migrations_snapshot_are_in_sync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            using var db = new AppDbContext(options);

            var sp = ((IInfrastructure<IServiceProvider>)db).Instance;
            var differ = sp.GetRequiredService<IMigrationsModelDiffer>();
            var assembly = sp.GetRequiredService<IMigrationsAssembly>();

            var design = db.GetService<IDesignTimeModel>();     
            var currentRel = design.Model.GetRelationalModel();

            var snapshotModel = assembly.ModelSnapshot?.Model
                ?? throw new InvalidOperationException("No model snapshot found. Add an initial migration.");
            var runtimeInit = db.GetService<IModelRuntimeInitializer>();
            var snapshotRel = runtimeInit.Initialize(snapshotModel, designTime: true).GetRelationalModel();

            var diffs = differ.GetDifferences(snapshotRel, currentRel);
            if (diffs.Count > 0)
            {
                var ops = string.Join(Environment.NewLine, diffs.Select(d => " - " + d.GetType().Name));
                throw new Xunit.Sdk.XunitException(
                    "EF model differs from migrations snapshot. Run:\n" +
                    "  dotnet ef migrations add <MeaningfulName>\n\n" +
                    "Detected operations:\n" + ops);
            }
        }


        [Fact]
        public async Task Invoices_table_has_index_on_CustomerId()
        {
            using var conn = new SqliteConnection("Data Source=:memory:");
            await conn.OpenAsync();

            var opts = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(conn).Options;
            using var db = new AppDbContext(opts);
            await db.Database.MigrateAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "PRAGMA index_list('Invoices');";
            using var r = await cmd.ExecuteReaderAsync();
            var names = new List<string>();
            while (await r.ReadAsync()) names.Add(r.GetString(1));

            names.Should().Contain(n => n.Contains("IX_Invoices_CustomerId"));
        }

        [Fact]
        public async Task Enforces_foreign_key_on_CustomerId()
        {
            using var conn = new SqliteConnection("Data Source=:memory:");
            await conn.OpenAsync();

            var opts = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(conn).Options;
            using var db = new AppDbContext(opts);
            await db.Database.MigrateAsync();

            var invoice = new Invoice { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), Total = 10m };

            db.Invoices.Add(invoice);

            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }
    }
}
