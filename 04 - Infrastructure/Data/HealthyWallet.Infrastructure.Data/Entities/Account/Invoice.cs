using HealthyWallet.Infrastructure.Data.Enums.Account;

namespace HealthyWallet.Infrastructure.Data.Entities.Account;

public class Invoice : EntityBase
{
    public required DateTime ReferenceMonth { get; set; }
    public required decimal TotalAmount { get; set; }
    public required decimal AmountPaid { get; set; }
    public required InvoiceStatus Status { get; set; }
    public required DateTime DueDate { get; set; }
    public required DateTime? PaymentDate { get; set; }

    // * relationships
    public required long CardId { get; set; }
    public Card? Card { get; set; }
    
    public ICollection<Transaction> Transactions { get; set; } =[];
}