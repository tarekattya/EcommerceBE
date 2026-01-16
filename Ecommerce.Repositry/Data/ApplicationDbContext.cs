

namespace Ecommerce.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options , IHttpContextAccessor httpContext):IdentityDbContext<ApplicationUser>(options)
{

    public ApplicationDbContext CreateDbContext(string[] args)
    { 
        return new ApplicationDbContext(
            options,
            new HttpContextAccessor()
        );
    }
    private readonly IHttpContextAccessor _httpContext = httpContext;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(GetIsDeletedFilter(entityType.ClrType));
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    private static LambdaExpression GetIsDeletedFilter(Type type)
    {
        var parameter = Expression.Parameter(type, "it");
        var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
        var falseConstant = Expression.Constant(false);
        var equal = Expression.Equal(property, falseConstant);
        return Expression.Lambda(equal, parameter);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken =default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(x => x.CreatedBy).CurrentValue = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                entry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.ModifiedAt).CurrentValue = DateTime.UtcNow;
                entry.Property(x => x.ModifiedBy).CurrentValue = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            else if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Property(x => x.IsDeleted).CurrentValue = true;
                entry.Property(x => x.ModifiedAt).CurrentValue = DateTime.UtcNow;
                entry.Property(x => x.ModifiedBy).CurrentValue = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(x => x.CreatedBy).CurrentValue = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                entry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.ModifiedAt).CurrentValue = DateTime.UtcNow;
                entry.Property(x => x.ModifiedBy).CurrentValue = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            else if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Property(x => x.IsDeleted).CurrentValue = true;
                entry.Property(x => x.ModifiedAt).CurrentValue = DateTime.UtcNow;
                entry.Property(x => x.ModifiedBy).CurrentValue = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
        }
        return base.SaveChanges();
    }
    public DbSet<ProductBrand> ProductBrands { get; set; } = default!;
    public DbSet<ProductCategory> ProductCategories { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<Core.Order> Orders { get; set; } = default!;
    public DbSet<OrderItem> OrderItems { get; set; } = default!;
    public DbSet<DeliveryMethod> DeliveryMethods { get; set; } = default!;

    
    public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;
    public DbSet<Coupon> Coupons { get; set; } = default!;



}
