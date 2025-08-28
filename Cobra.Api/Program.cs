using Microsoft.AspNetCore.SpaServices.Extensions;

namespace Cobra.Api;


public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseStaticFiles();
        app.MapControllers();
        app.MapFallbackToFile("/index.hml");

        app.UseHttpsRedirection();
        app.UseAuthorization();


        app.Run();
    }
}
