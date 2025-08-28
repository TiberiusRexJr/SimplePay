using Cobra.Api.Data;
using Cobra.Api.Data.Seed;
using Microsoft.AspNetCore.SpaServices.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Cobra.Api;


public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));


        var app = builder.Build();
        Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "AppData"));


        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
            await AppDbSeeder.EnsureSeeded(db); // make this idempotent
        }


        //if (app.Environment.IsDevelopment())
        //{
        //    app.UseDeveloperExceptionPage();
           
        //}

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapControllers();
        await app.RunAsync();

    }
}
