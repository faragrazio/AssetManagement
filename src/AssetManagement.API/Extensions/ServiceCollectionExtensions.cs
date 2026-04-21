using System.Text;
using AssetManagement.Application.Assets.Commands.CreateAsset;
using AssetManagement.Application.Common.Behaviors;
using AssetManagement.Application.Common.Interfaces;
using AssetManagement.Domain.Interfaces;
using AssetManagement.Infrastructure.Identity;
using AssetManagement.Infrastructure.Persistence;
using AssetManagement.Infrastructure.Persistence.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AssetManagement.API.Extensions;

public static class ServiceCollectionExtensions
{
    // Registra DbContext con PostgreSQL
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AssetManagementDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(
                    typeof(AssetManagementDbContext).Assembly.FullName)
            ));

        // Registra IApplicationDbContext → AssetManagementDbContext
        services.AddScoped<IApplicationDbContext>(
            provider => provider.GetRequiredService<AssetManagementDbContext>());

        return services;
    }

    // Registra MediatR + Behaviors + Validators
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // MediatR — scansiona l'assembly di Application per trovare tutti gli Handler
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(
                typeof(CreateAssetCommand).Assembly));

        // Pipeline behaviors — ordine importante: Logging → Validation → Handler
        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        // FluentValidation — registra tutti i validator dall'assembly Application
        services.AddValidatorsFromAssembly(
            typeof(CreateAssetCommand).Assembly);

        return services;
    }

    // Registra Repository e servizi Infrastructure
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IMaintenanceOrderRepository, MaintenanceOrderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }

// Configura autenticazione JWT
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key non configurata.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;
    }

    // Configura Swagger base
    public static IServiceCollection AddSwagger(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}