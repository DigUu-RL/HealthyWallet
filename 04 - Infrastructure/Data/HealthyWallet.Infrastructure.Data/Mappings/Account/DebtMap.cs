using HealthyWallet.Infrastructure.Data.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthyWallet.Infrastructure.Data.Mappings.Account;

public sealed class DebtMap : BaseMap<Debt>
{
    public override void Configure(EntityTypeBuilder<Debt> builder)
    {
        builder.HasIndex(debt => debt.UserId);
        builder.HasIndex(debt => debt.NextDueDate);
        builder.HasIndex(debt => debt.Status);

        builder.Property(debt => debt.Title).HasMaxLength(100).IsRequired();
        builder.Property(debt => debt.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(debt => debt.Installments).IsRequired();
        builder.Property(debt => debt.RemainingInstallments).IsRequired();
        builder.Property(debt => debt.NextDueDate).IsRequired();
        builder.Property(debt => debt.Status).HasConversion<string>().IsRequired();

        // relationships
        builder
            .HasOne(debt => debt.User)
            .WithMany(user => user.Debts)
            .HasForeignKey(debt => debt.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        base.Configure(builder);
    }
}