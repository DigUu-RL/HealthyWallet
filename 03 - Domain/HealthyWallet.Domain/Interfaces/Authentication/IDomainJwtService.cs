

using HealthyWallet.Domain.Models.Authentication;
using HealthyWallet.Infrastructure.Data.Entities.Identity;

namespace HealthyWallet.Domain.Interfaces.Authentication;

public interface IDomainJwtService
{
    AccessTokenModel GenerateToken(User user);
    Task<User> ValidateToken(string token);
}