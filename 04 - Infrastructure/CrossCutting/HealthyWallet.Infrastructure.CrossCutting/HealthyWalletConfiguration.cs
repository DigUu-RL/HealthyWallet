using HealthyWallet.Infrastructure.CrossCutting.Middlewares;
using HealthyWallet.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthyWallet.Infrastructure.CrossCutting;

public static class HealthyWalletConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
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
                    $"Missing connection string for '{connectorKey}'"
                );
            }
            
            if (usePostgres) options.UseNpgsql(connectionString);
            else options.UseSqlServer(connectionString);

            options.UseLazyLoadingProxies(false);
        });

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
        app.UseMiddleware<DbTransactionMiddleware>();

        return app;
    }
}