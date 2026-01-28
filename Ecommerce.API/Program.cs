namespace Ecommerce.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder?  builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationServices(builder.Configuration , builder);

        WebApplication? app = builder.Build();

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
        app.UseHttpsRedirection();

        if (!app.Environment.IsDevelopment())
        {
            app.UseCors("AllowSpecificOrigin");
        }

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseStatusCodePagesWithReExecute("/Errors/{0}");
        app.UseStaticFiles();

        app.MapControllers();

        app.Run();
    }
}
