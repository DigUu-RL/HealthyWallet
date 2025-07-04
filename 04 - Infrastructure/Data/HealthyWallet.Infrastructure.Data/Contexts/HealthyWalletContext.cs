using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HealthyWallet.Infrastructure.Data.Contexts;

public class HealthyWalletContext(DbContextOptions<HealthyWalletContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HealthyWalletContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}