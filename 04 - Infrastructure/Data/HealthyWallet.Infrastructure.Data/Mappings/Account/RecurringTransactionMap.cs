using HealthyWallet.Infrastructure.Data.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthyWallet.Infrastructure.Data.Mappings.Account;

public sealed class RecurringTransactionMap : BaseMap<RecurringTransaction>
{
    public override void Configure(EntityTypeBuilder<RecurringTransaction> builder)
    {
        builder.Property(recurring => recurring.Frequency).HasConversion<string>().IsRequired();
        builder.Property(recurring =>  recurring.EndDate).IsRequired(false);

        // relationships
        builder
            .HasOne(recurring => recurring.Transaction)
            .WithOne(transaction => transaction.RecurringTransaction)
            .HasForeignKey<RecurringTransaction>(recurring => recurring.TransactionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        base.Configure(builder);
    }
}