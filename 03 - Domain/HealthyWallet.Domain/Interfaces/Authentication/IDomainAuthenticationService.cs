using HealthyWallet.Domain.Models.Authentication;
using HealthyWallet.Domain.Requests.Authentication;
using HealthyWallet.Infrastructure.Data.Entities.Identity;

namespace HealthyWallet.Domain.Interfaces.Authentication;

public interface IDomainAuthenticationService
{
    Task<AccessTokenModel> SignIn(SignInRequest model);
    Task<User> ValidateToken(string type, string token);
}