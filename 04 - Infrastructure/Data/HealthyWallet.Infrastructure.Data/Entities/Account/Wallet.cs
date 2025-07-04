using HealthyWallet.Infrastructure.Data.Entities.Identity;
using HealthyWallet.Infrastructure.Data.Enums;

namespace HealthyWallet.Infrastructure.Data.Entities.Account;

public class Wallet : EntityBase
{
    public required string Name { get; set; }
    public required Currency Currency { get; set; }
    public required decimal Balance { get; set; }
    public required bool IsActive { get; set; }

    // * relationships
    public required long UserId { get; set; }
    public User? User { get; set; }
    
    public ICollection<Transaction> Transactions { get; set; } = [];
}