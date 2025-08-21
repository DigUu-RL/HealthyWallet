using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;
using HealthyWallet.Infrastructure.Data.Entities.Account;
using HealthyWallet.Infrastructure.Data.Enums.Identity;

namespace HealthyWallet.Infrastructure.Data.Entities.Identity;

public class User : EntityBase, IIdentity
{
    public required string Name { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required UserType Type { get; set; } 

    [NotMapped] public string? AuthenticationType { get; set; }
    [NotMapped] public bool IsAuthenticated { get; set; }

    // * relationships
    public ICollection<UserClaim> Claims { get; set; } = [];
    public ICollection<Wallet> Wallets { get; set; } = [];
    public ICollection<Card> Cards { get; set; } = [];
    public ICollection<Category> Categories { get; set; } = [];
    public ICollection<Debt> Debts { get; set; } = [];
}