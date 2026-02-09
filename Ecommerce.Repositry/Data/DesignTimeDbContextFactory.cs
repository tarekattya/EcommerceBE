public class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=.;Database=Ecommerce;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        return new ApplicationDbContext(
            options,
            new HttpContextAccessor() 
        );
    }
}
