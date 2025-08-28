using Cobra.Api.Data;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cobra.Api.Tests.Data
{
    public class MigrationsTests
    {
        [Fact]
        public async Task Can_apply_all_migrations_without_error()
        {
            using var conn = new SqliteConnection("Data Source=:memory:");
            await conn.OpenAsync();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(conn)
                .Options;

            using var db = new AppDbContext(options);

            await db.Database.MigrateAsync();

            var pending = await db.Database.GetPendingMigrationsAsync();
            pending.Should().BeEmpty();
        }
    }
}
