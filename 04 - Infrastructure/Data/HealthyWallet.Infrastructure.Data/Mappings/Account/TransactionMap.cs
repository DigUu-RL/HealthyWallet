using HealthyWallet.Infrastructure.Data.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthyWallet.Infrastructure.Data.Mappings.Account;

public sealed class TransactionMap : BaseMap<Transaction>
{
    public override void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasIndex(transaction => new { transaction.WalletId, transaction.Date });
        builder.HasIndex(transaction => transaction.CategoryId);
        builder.HasIndex(transaction => transaction.Type);
        
        builder.Property(transaction => transaction.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(transaction => transaction.Type).HasConversion<string>().IsRequired();
        builder.Property(transaction => transaction.Description).IsRequired(false);
        builder.Property(transaction => transaction.Date).IsRequired();
        builder.Property(transaction => transaction.IsRecurring).IsRequired();
        
        // relationships
        builder
            .HasOne(transaction => transaction.Wallet)
            .WithMany(wallet => wallet.Transactions)
            .HasForeignKey(transaction => transaction.WalletId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasOne(transaction => transaction.Category)
            .WithMany(category => category.Transactions)
            .HasForeignKey(transaction => transaction.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasOne(transaction => transaction.Card)
            .WithMany(card => card.Transactions)
            .HasForeignKey(transaction => transaction.CardId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasOne(transaction => transaction.Invoice)
            .WithMany(invoice => invoice.Transactions)
            .HasForeignKey(transaction => transaction.InvoiceId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasOne(transaction => transaction.RecurringTransaction)
            .WithOne(recurring => recurring.Transaction)
            .HasForeignKey<RecurringTransaction>(recurring => recurring.TransactionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        base.Configure(builder);
    }
}