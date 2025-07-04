namespace HealthyWallet.Infrastructure.Data.Entities;

public abstract class EntityBase
{
    public long Id { get; set; }
    public Guid ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    protected void InitRequiredFields()
    {
        ReferenceId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    protected void UpdateTimestamp() => UpdatedAt = DateTime.UtcNow;
}