
using Ecommerce.API.MiddleWares;
using Ecommerce.Infrastructure;
using Ecommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddApplicationServices(builder.Configuration , builder);



            var app = builder.Build();

            await DbInitializer.InitializeAsync(app);


            app.UseExceptionMiddleware();
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(op =>
                {
                    op.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }
            app.UseStatusCodePagesWithReExecute("/Errors/{0}");
            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
