using HealthyWallet.Infrastructure.Data.Enums.Account;

namespace HealthyWallet.Infrastructure.Data.Entities.Account;

public class RecurringTransaction : EntityBase
{
    public required Frequency Frequency { get; set; }
    public DateTime? EndDate { get; set; }

    // * relationships
    public required long TransactionId { get; set; }
    public Transaction? Transaction { get; set; }
}