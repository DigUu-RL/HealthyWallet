using HealthyWallet.Infrastructure.Data.Entities.Identity;
using HealthyWallet.Infrastructure.Data.Enums.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthyWallet.Infrastructure.Data.Mappings.Identity;

public sealed class UserMap : BaseMap<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(user => user.UserName).IsUnique();
        builder.HasIndex(user => user.Email).IsUnique();

        builder.Property(user => user.Name).HasMaxLength(100).IsRequired();
        builder.Property(user => user.UserName).HasMaxLength(150).IsRequired();
        builder.Property(user => user.Email).HasMaxLength(250).IsRequired();
        builder.Property(user => user.PasswordHash).HasMaxLength(250).IsRequired();
        builder.Property(user => user.Type).HasConversion<string>().IsRequired();

        // * relationships
        builder
            .HasMany(user => user.Claims)
            .WithOne(claim => claim.User)
            .HasForeignKey(claim => claim.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(user => user.Wallets)
            .WithOne(wallet => wallet.User)
            .HasForeignKey(wallet => wallet.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(user => user.Cards)
            .WithOne(card => card.User)
            .HasForeignKey(card => card.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(user => user.Categories)
            .WithOne(category => category.User)
            .HasForeignKey(category => category.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(user => user.Debts)
            .WithOne(debt => debt.User)
            .HasForeignKey(debt => debt.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new User
            {
                Id = 1,
                ReferenceId = Guid.Parse("0794F545-C151-4F1A-8501-A5F63175707C"),
                Name = "Rodrigo Lombardo",
                UserName = "diguu",
                Email = "rodrigogeribola@hotmail.com",
                PasswordHash = "$2b$12$6HaTQTnwfStBBf3VKfOc2.2Jh93jnrVEpXI0Sso2aTH5kTa5mFruS",
                Type = UserType.Administrator,
                CreatedAt = DateTime.Parse("08/06/2025 20:43:15"),
                UpdatedAt = DateTime.Parse("08/06/2025 20:43:15")
            }
        );

        base.Configure(builder);
    }
}