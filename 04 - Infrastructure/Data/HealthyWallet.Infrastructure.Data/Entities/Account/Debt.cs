using HealthyWallet.Infrastructure.Data.Entities.Identity;
using HealthyWallet.Infrastructure.Data.Enums.Account;

namespace HealthyWallet.Infrastructure.Data.Entities.Account;

public class Debt : EntityBase
{
    public required string Title { get; set; }
    public required decimal Amount { get; set; }
    public required int Installments { get; set; }
    public required int RemainingInstallments { get; set; }
    public DateTime? NextDueDate { get; set; }
    public required DebtStatus Status { get; set; }

    // * relationships
    public required long UserId { get; set; }
    public User? User { get; set; }
}