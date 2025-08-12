using HealthyWallet.Infrastructure.Data.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthyWallet.Infrastructure.Data.Mappings.Account;

public sealed class WalletMap : BaseMap<Wallet>
{
    public override void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.HasIndex(wallet => new { wallet.UserId, wallet.Name }).IsUnique();
        builder.HasIndex(wallet => wallet.UserId);
        
        builder.Property(wallet => wallet.Name).HasMaxLength(50).IsRequired();
        builder.Property(wallet => wallet.Currency).HasConversion<string>().IsRequired();
        builder.Property(wallet => wallet.Balance).HasPrecision(18, 2).IsRequired();
        builder.Property(wallet => wallet.IsActive).IsRequired();
        
        // relationships
        builder
            .HasOne(wallet => wallet.User)
            .WithMany(user => user.Wallets)
            .HasForeignKey(wallet => wallet.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasMany(wallet => wallet.Transactions)
            .WithOne(transaction => transaction.Wallet)
            .HasForeignKey(transaction => transaction.WalletId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        base.Configure(builder);
    }
}