using HealthyWallet.Infrastructure.Data.Enums.Account;

namespace HealthyWallet.Infrastructure.Data.Entities.Account;

public class Transaction : EntityBase
{
    public required decimal Amount { get; set; }
    public required TransactionType Type { get; set; }
    public string? Description { get; set; }
    public required DateTime Date { get; set; }
    public required bool IsRecurring { get; set; }

    // * relationships
    public required long WalletId { get; set; }
    public Wallet? Wallet { get; set; }
    
    public required long CategoryId { get; set; }
    public Category? Category { get; set; }
    
    public long? CardId { get; set; }
    public Card? Card { get; set; }
    
    public long? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    
    public RecurringTransaction? RecurringTransaction { get; set; }
}
