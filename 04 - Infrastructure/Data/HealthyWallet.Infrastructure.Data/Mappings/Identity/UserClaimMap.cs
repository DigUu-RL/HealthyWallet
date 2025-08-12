using HealthyWallet.Infrastructure.Data.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthyWallet.Infrastructure.Data.Mappings.Identity;

public sealed class UserClaimMap : BaseMap<UserClaim>
{
    public override void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.Property(claim => claim.Type).IsRequired().HasMaxLength(100);
        builder.Property(claim => claim.Value).IsRequired().HasMaxLength(255);

        // relationships
        builder
            .HasOne(claim => claim.User)
            .WithMany(user => user.Claims)
            .HasForeignKey(claim => claim.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        base.Configure(builder);
    }
}