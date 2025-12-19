public class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=Ecommerce.db")
            .Options;

        return new ApplicationDbContext(
            options,
            new HttpContextAccessor() 
        );
    }
}
