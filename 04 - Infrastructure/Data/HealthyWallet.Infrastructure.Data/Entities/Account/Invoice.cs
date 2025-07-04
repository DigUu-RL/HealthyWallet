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

    public void UpdateStatus(DateTime currentDate)
    {
        if (AmountPaid >= TotalAmount)
        {
            Status = InvoiceStatus.Paid;
            PaymentDate = DateTime.UtcNow;

            return;
        }

        if (AmountPaid > 0 && AmountPaid < TotalAmount)
        {
            Status = currentDate > DueDate ? InvoiceStatus.Overdue : InvoiceStatus.PartiallyPaid;
            return;
        }

        Status = currentDate > DueDate ? InvoiceStatus.Overdue : InvoiceStatus.Open;
    }
}