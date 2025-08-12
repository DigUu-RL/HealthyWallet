using HealthyWallet.Infrastructure.Data.Entities.Identity;

namespace HealthyWallet.Infrastructure.Data.Entities.Account;

public class Category : EntityBase
{
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public string? ColorHex { get; set; }
    public bool IsDefault { get; set; }

    // * relationships
    public long? UserId { get; set; }
    public User? User { get; set; }
    
    public ICollection<Transaction> Transactions { get; set; } = [];
}