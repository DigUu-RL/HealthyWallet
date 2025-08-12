using HealthyWallet.Infrastructure.Data.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthyWallet.Infrastructure.Data.Mappings.Account;

public sealed class InvoiceMap : BaseMap<Invoice>
{
    public override void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasIndex(invoice => invoice.CardId);
        builder.HasIndex(invoice => invoice.ReferenceMonth);
        builder.HasIndex(invoice => invoice.Status);

        builder.Property(invoice => invoice.ReferenceMonth).IsRequired();
        builder.Property(invoice => invoice.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(invoice => invoice.AmountPaid).HasPrecision(18, 2).IsRequired();
        builder.Property(invoice => invoice.Status).HasConversion<string>().IsRequired();
        builder.Property(invoice => invoice.DueDate).IsRequired();
        builder.Property(invoice => invoice.PaymentDate).IsRequired();
        
        // relationships
        builder
            .HasOne(invoice => invoice.Card)
            .WithMany(card => card.Invoices)
            .HasForeignKey(invoice => invoice.CardId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasMany(invoice => invoice.Transactions)
            .WithOne(transaction => transaction.Invoice)
            .HasForeignKey(transaction => transaction.InvoiceId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        base.Configure(builder);
    }
}