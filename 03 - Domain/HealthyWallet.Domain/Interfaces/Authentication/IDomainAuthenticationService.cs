using HealthyWallet.Domain.Models.Authentication;
using HealthyWallet.Domain.Requests.Authentication;

namespace HealthyWallet.Domain.Interfaces.Authentication;

public interface IDomainAuthenticationService
{
    Task<AccessTokenModel> SignIn(SignInRequest model);
    Task ValidateToken(string type, string token);
}