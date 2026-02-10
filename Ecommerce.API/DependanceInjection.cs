using Newtonsoft.Json.Converters;
using Ecommerce.Core;
using Ecommerce.Services;
using Ecommerce.Application;
using Hangfire;
using Hangfire.SqlServer;

namespace Ecommerce.API;

public static class DependanceInjection
{

   
    public static IServiceCollection AddApplicationServices(this IServiceCollection services ,IConfiguration configuration,WebApplicationBuilder webApplication)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", policy =>
            {
                policy.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "https://localhost:7021" })
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddMapping(configuration);
        services.AddValidtionResponse(webApplication);
        services.AddFluentValidation();
        services.AddIdentity(configuration);


        services.AddDBServices(configuration);

        // Health checks (for load balancers, containers, monitoring)
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("database", tags: new[] { "ready" })
            .AddRedis(configuration.GetConnectionString("RedisConnection")!, "redis", tags: new[] { "ready" });

        var hangfireConnection = configuration.GetConnectionString("HangfireConnection");
        if (string.IsNullOrWhiteSpace(hangfireConnection))
            hangfireConnection = configuration.GetConnectionString("Data");

        if (!string.IsNullOrWhiteSpace(hangfireConnection))
        {
            var storageOptions = new SqlServerStorageOptions
            {
                PrepareSchemaIfNecessary = true,
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5)
            };
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(hangfireConnection, storageOptions));

            services.AddHangfireServer();
        }
        
        // Hangfire job classes (resolved by Hangfire when running jobs)
        services.AddScoped<DailyCleanupJob>();
        
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductVariantService, ProductVariantService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IDashboardService, DashboardService>();
        
        // Email services - Hangfire is the only background engine
        services.AddScoped<EmailService>();                 // Direct SMTP sender
        services.AddScoped<IEmailService, HangfireEmailService>(); // IEmailService implementation that enqueues Hangfire jobs
        
        services.AddSingleton<ICacheService, CacheService>();
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<IRatingService, RatingService>();
        services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
        services.Configure<PaymobSettings>(configuration.GetSection(PaymobSettings.SectionName));
        services.AddScoped<IPaymentService, PaymobPaymentService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDomainEventHandler<OrderStockReleaseEvent>, OrderStockReleaseHandler>();
        services.AddScoped<IDomainEventHandler<OrderProcessingStartedEvent>, OrderProcessingStartedHandler>();
        services.AddScoped<IDomainEventHandler<OrderCreatedEvent>, OrderCreatedEmailHandler>();
        services.AddScoped<IDomainEventHandler<OrderPaymentSucceededEvent>, OrderPaymentSucceededInvoiceHandler>();
        services.AddScoped<IDomainEventHandler<OrderProcessingStartedEvent>, OrderProcessingStartedInvoiceHandler>();
        services.AddScoped<IDomainEventHandler<OrderCancelledEvent>, OrderCancelledEmailHandler>();
        services.AddScoped<IDomainEventHandler<OrderShippedEvent>, OrderShippedEmailHandler>();
        services.AddScoped<IDomainEventHandler<OrderDeliveredEvent>, OrderDeliveredEmailHandler>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ISavedAddressService, SavedAddressService>();
        services.AddHttpClient();

        return services;
    }

    public static IServiceCollection AddDBServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(
            options =>
            {
                options/*.UseLazyLoadingProxies()*/.UseSqlServer(configuration.GetConnectionString("Data"));
            });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        { 
            return ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")!);
        });
        return services;
    }
    public static IServiceCollection AddMapping(this IServiceCollection services, IConfiguration configuration)
    {

        services.Configure<BaseUrl>(configuration.GetSection("BaseUrl"));

        TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
        using IServiceScope scope = services.BuildServiceProvider().CreateScope();
        IOptions<BaseUrl> urlOptions = scope.ServiceProvider.GetRequiredService<IOptions<BaseUrl>>();
        Config.Register(config, urlOptions);
        services.AddSingleton<IMapper>(sp => new Mapper(TypeAdapterConfig.GlobalSettings));

        return services;
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {

        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        services.AddFluentValidationAutoValidation();


        return services;
    }

    public static IServiceCollection AddIdentity(this IServiceCollection services , IConfiguration configuration)
    {

        
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddSingleton<IJwtProvider, JwtProvider>();

        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }


        ).AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.Key)),

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience
            };
            
            o.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    
                    var errorResponse = new Error("Unauthorized", "Authentication failed. Please provide a valid token.", StatusCodes.Status401Unauthorized);
                    var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                    var json = JsonSerializer.Serialize(errorResponse, options);
                    
                    return context.Response.WriteAsync(json);
                },
                OnAuthenticationFailed = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }
    public static IServiceCollection AddValidtionResponse(this IServiceCollection services , WebApplicationBuilder webApplication)
    {
        webApplication.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = (actionContext) =>
            {
                var errors = actionContext.ModelState
                    .Where(e => e.Value!.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage).ToList();

                var errorResponse = new ApiValidationResponse
                {
                    Errors = errors
                };

                return new BadRequestObjectResult(errorResponse);
            };
        });

        return services;
    }
}
