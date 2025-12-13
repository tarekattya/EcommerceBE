namespace Ecommerce.API;

public static class DependanceInjection
{

   
    public static IServiceCollection AddApplicationServices(this IServiceCollection services ,IConfiguration configuration,WebApplicationBuilder webApplication)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddMapping(configuration);
        services.AddValidtionResponse(webApplication);
        services.AddFluentValidation();
        services.AddIdentity(configuration);


        services.AddDBServices(configuration);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICartService, CartService>();
        return services;
    }

    public static IServiceCollection AddDBServices(this IServiceCollection services , IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(
            options =>
            {
                options/*.UseLazyLoadingProxies()*/.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
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

        var config = TypeAdapterConfig.GlobalSettings;
        using var scope = services.BuildServiceProvider().CreateScope();
        var urlOptions = scope.ServiceProvider.GetRequiredService<IOptions<BaseUrl>>();
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
