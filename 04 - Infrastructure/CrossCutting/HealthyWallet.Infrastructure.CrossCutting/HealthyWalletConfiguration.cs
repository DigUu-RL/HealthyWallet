using System.Text;
using HealthyWallet.Infrastructure.CrossCutting.Middlewares;
using HealthyWallet.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HealthyWallet.Application.Interfaces.Authentication;
using HealthyWallet.Application.Services.Authentication;
using HealthyWallet.Domain.Interfaces.Authentication;
using HealthyWallet.Domain.Services.Authentication;
using HealthyWallet.Infrastructure.Repository.Interfaces;
using HealthyWallet.Infrastructure.Repository.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace HealthyWallet.Infrastructure.CrossCutting;

/// <summary>
/// Provides extension methods for configuring HealthyWallet application services,
/// domain services, repositories, database contexts, singletons, and middlewares.
/// </summary>
public static class HealthyWalletConfiguration
{
    /// <summary>
    /// Registers application-level services into the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the services into.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the application services registered.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IApplicationAuthenticationService, ApplicationAuthenticationService>();

        return services;
    }

    /// <summary>
    /// Registers domain-level services into the dependency injection container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the services into.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the domain services registered.</returns>
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IDomainAuthenticationService, DomainAuthenticationService>();
        services.AddScoped<IDomainJwtService, DomainJwtService>();

        return services;
    }

    /// <summary>
    /// Registers repository implementations into the dependency injection container.
    /// Includes generic base repositories and dynamically scans for additional repository implementations.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the repositories into.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the repositories registered.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped(typeof(IReadOnlyRepository<>), typeof(BaseRepository<>));

        services.Scan(selector => selector
            .FromAssembliesOf(typeof(IBaseRepository<>))
            .AddClasses(filter =>
                filter.AssignableTo(typeof(IBaseRepository<>)).Where(type => type is
                {
                    IsAbstract: false,
                    IsGenericTypeDefinition: false
                })
            )
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        return services;
    }

    /// <summary>
    /// Registers the application's <see cref="DbContext"/> into the dependency injection container,
    /// based on the configuration provided.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the DbContext to.</param>
    /// <param name="configuration">The application configuration used to retrieve connection string settings.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the DbContext registered.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="configuration"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the expected connection string is missing from configuration.</exception>
    public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        services.AddDbContext<HealthyWalletContext>(options =>
        {
            bool usePostgres = configuration.GetSection("Databases").GetValue<bool>("UsePostgres");

            string connectorKey = usePostgres ? "Postgres" : "SqlServer";
            string? connectionString = configuration.GetConnectionString(connectorKey);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    $"Missing connection string for '{connectorKey}' key"
                );
            }

            if (usePostgres) options.UseNpgsql(connectionString);
            else options.UseSqlServer(connectionString);

            options.UseLazyLoadingProxies(false);
        });

        return services;
    }

    /// <summary>
    /// Registers singleton dependencies required by the application, including JWT token validation parameters.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the singletons into.</param>
    /// <param name="configuration">The application configuration used to retrieve dependency settings.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the singleton dependencies registered.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if required JWT settings (Issuer, Audience, or SecretKey) are missing from configuration.
    /// </exception>
    public static IServiceCollection AddSingletonDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        IConfigurationSection jwtSection = configuration.GetSection("Jwt");
        
        string issuer = jwtSection.GetValue<string>("Issuer") ?? throw new InvalidOperationException("Issuer not found");
        string audience = jwtSection.GetValue<string>("Audience") ?? throw new InvalidOperationException("Audience not found");
        string secretKey = jwtSection.GetValue<string>("SecretKey") ?? throw new InvalidOperationException("Secret key not found");

        services.AddSingleton(new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        });
        
        return services;
    }

    /// <summary>
    /// Registers custom middlewares into the dependency injection container for use in the application's request pipeline.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the middlewares into.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the middlewares registered.</returns>
    public static IServiceCollection AddMiddlewares(this IServiceCollection services)
    {
        services.AddTransient<GlobalExceptionMiddleware>();
        services.AddTransient<JwtMiddleware>();
        services.AddTransient<DbTransactionMiddleware>();
        
        return services;
    }

    /// <summary>
    /// Adds custom middlewares to the application's request pipeline, including
    /// global exception handling and database transaction handling.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> used to configure the middleware pipeline.</param>
    /// <returns>The updated <see cref="IApplicationBuilder"/> with the custom middlewares added.</returns>
    public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        
        app.UseWhen(
            context => !(context.GetEndpoint()?.Metadata.OfType<AllowAnonymousAttribute>().Any() ?? false),
            builder => builder.UseMiddleware<JwtMiddleware>()
        );
        
        app.UseMiddleware<DbTransactionMiddleware>();

        return app;
    }
}