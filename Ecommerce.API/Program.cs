
using Ecommerce.API.Data;
using Ecommerce.Repositry.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddApplicationServices(builder.Configuration);


            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerfactory = app.Services.GetRequiredService<ILoggerFactory>(); 
            var context = services.GetRequiredService<ApplicationDbContext>();
            try
            {
               await  context.Database.MigrateAsync();
                await ApplicationDbSeeding.SeedAsync(context);
            }
            catch (Exception ex)
            {
                var logger = loggerfactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
