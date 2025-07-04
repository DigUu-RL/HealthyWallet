using HealthyWallet.Infrastructure.Data.Entities.Identity;

namespace HealthyWallet.Infrastructure.Data.Entities.Account;

public class Card : EntityBase
{
    public required string Name { get; set; } 
    public required string Brand { get; set; }
    public required decimal Limit { get; set; }
    public required int DueDay { get; set; }
    public required int ClosingDay { get; set; }
    public required bool IsActive { get; set; }

    // * relationships
    public required long UserId { get; set; }
    public User? User { get; set; }
    
    public ICollection<Invoice> Invoices { get; set; } = [];
    public ICollection<Transaction> Transactions { get; set; } = [];
}