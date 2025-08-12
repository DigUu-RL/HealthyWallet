using HealthyWallet.Infrastructure.Data.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthyWallet.Infrastructure.Data.Mappings.Account;

public sealed class CategoryMap : BaseMap<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasIndex(category => category.UserId);
        builder.HasIndex(category => new { category.UserId, category.Name }).IsUnique();

        builder.Property(category => category.Name).HasMaxLength(100).IsRequired();
        builder.Property(category => category.Icon).HasMaxLength(100).IsRequired(false);
        builder.Property(category => category.ColorHex).HasMaxLength(7).IsRequired(false);
        builder.Property(category => category.IsDefault).IsRequired();
        builder.Property(category => category.UserId).IsRequired(false);

        // relationships
        builder
            .HasOne(category => category.User)
            .WithMany(user => user.Categories)
            .HasForeignKey(category => category.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany(category => category.Transactions)
            .WithOne(transaction => transaction.Category)
            .HasForeignKey(transaction => transaction.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasData(
            new Category
            {
                Id = 1,
                ReferenceId = Guid.Parse("ACCD652B-6FB3-434B-949F-F2C606A1C580"),
                Name = "Não especificado",
                Icon = "n/a",
                ColorHex = "#808080",
                IsDefault = true,
                UserId = null,
                CreatedAt = DateTime.Parse("08/06/2025 20:43:15"),
                UpdatedAt = DateTime.Parse("08/06/2025 20:43:15")
            }
        );

        base.Configure(builder);
    }
}