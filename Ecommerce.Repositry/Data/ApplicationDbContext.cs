

namespace Ecommerce.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options , IHttpContextAccessor httpContext):IdentityDbContext<ApplicationUser>(options)
{
    private readonly IHttpContextAccessor _httpContext = httpContext;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken =default)
    {
        var entries = ChangeTracker.Entries<AuditLogging>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(x => x.CreatedBy).CurrentValue = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.ModifiedAt).CurrentValue = DateTime.UtcNow;
                entry.Property(x => x.ModifiedBy).CurrentValue = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
    public DbSet<ProductBrand> ProductBrands { get; set; } = default!;
    public DbSet<ProductCategory> ProductCategories { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;



}
