using HealthyWallet.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthyWallet.Infrastructure.Data.Mappings;

public abstract class BaseMap<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : EntityBase
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(entity => entity.Id);
        builder.HasIndex(entity => entity.ReferenceId).IsUnique();

        builder.Property(entity => entity.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(entity => entity.ReferenceId).IsRequired();
        builder.Property(entity => entity.CreatedAt).IsRequired();
        builder.Property(entity => entity.UpdatedAt).IsRequired();
    }
}