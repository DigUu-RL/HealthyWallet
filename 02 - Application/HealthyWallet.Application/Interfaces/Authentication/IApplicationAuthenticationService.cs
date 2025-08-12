using HealthyWallet.Application.DTOs;
using HealthyWallet.Domain.Requests.Authentication;

namespace HealthyWallet.Application.Interfaces.Authentication;

public interface IApplicationAuthenticationService
{
    Task<AccessTokenDto> SignIn(SignInRequest request);
    Task ValidateToken(string token);
}