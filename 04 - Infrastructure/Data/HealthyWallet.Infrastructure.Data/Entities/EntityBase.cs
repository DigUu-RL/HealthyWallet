namespace HealthyWallet.Infrastructure.Data.Entities;

public abstract class EntityBase
{
    public long Id { get; set; }
    public Guid ReferenceId { get; set; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public void UpdateTimestamp() => UpdatedAt = DateTime.UtcNow;
}