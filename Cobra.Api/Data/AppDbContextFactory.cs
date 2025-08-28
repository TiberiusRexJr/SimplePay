using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace Cobra.Api.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Ensure the folder for the SQLite file exists
            var dbFolder = Path.Combine(Directory.GetCurrentDirectory(), "AppData");
            if (!Directory.Exists(dbFolder)) Directory.CreateDirectory(dbFolder);

            var connectionString = "Data Source=AppData/cobra.db";

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connectionString)
                .Options;

            return new AppDbContext(options);
        }
    }
}
