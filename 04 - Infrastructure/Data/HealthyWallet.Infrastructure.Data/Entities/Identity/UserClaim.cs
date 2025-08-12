using System.Security.Claims;

namespace HealthyWallet.Infrastructure.Data.Entities.Identity;

public class UserClaim : EntityBase
{
    public string Type { get; set; }
    public string Value { get; set; }

    // * relationships
    public long UserId { get; set; }
    public User? User { get; set; }

    public UserClaim(string type, string value)
    {
        if (string.IsNullOrEmpty(type)) throw new ArgumentNullException(nameof(type));
        if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

        Type = type;
        Value = value;
    }

    public static implicit operator Claim(UserClaim claim) => new(claim.Type, claim.Value);
    public static implicit operator UserClaim(Claim claim) => new(claim.Type, claim.Value);
}