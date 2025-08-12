using HealthyWallet.Infrastructure.Data.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthyWallet.Infrastructure.Data.Mappings.Account;

public sealed class CardMap : BaseMap<Card>
{
    public override void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.Property(card => card.Name).HasMaxLength(50).IsRequired();
        builder.Property(card => card.Brand).HasConversion<string>().IsRequired();
        builder.Property(card => card.Limit).HasPrecision(18, 2).IsRequired();
        builder.Property(card => card.DueDay).IsRequired();
        builder.Property(card => card.ClosingDay).IsRequired();
        builder.Property(card => card.IsActive).IsRequired();

        // relationships
        builder
            .HasOne(card => card.User)
            .WithMany(user => user.Cards)
            .HasForeignKey(card => card.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany(card => card.Invoices)
            .WithOne(invoice => invoice.Card)
            .HasForeignKey(invoice => invoice.CardId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasMany(card => card.Transactions)
            .WithOne(transaction => transaction.Card)
            .HasForeignKey(transaction => transaction.CardId)
            .OnDelete(DeleteBehavior.NoAction);
        
        base.Configure(builder);
    }
}