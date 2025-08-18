using HealthyWallet.Application.DTOs;
using HealthyWallet.Application.Interfaces.Authentication;
using HealthyWallet.Domain.Exceptions.Abstractions.Authentication;
using HealthyWallet.Domain.Interfaces.Authentication;
using HealthyWallet.Domain.Models.Authentication;
using HealthyWallet.Domain.Requests.Authentication;

namespace HealthyWallet.Application.Services.Authentication;

public class ApplicationAuthenticationService(IDomainAuthenticationService authenticationService) : IApplicationAuthenticationService
{
    public async Task<AccessTokenDto> SignIn(SignInRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidCredentialsException("Invalid username or password");
        
        AccessTokenModel model = await authenticationService.SignIn(request);
        return new AccessTokenDto(model);
    }

    public async Task ValidateToken(string token)
    {
        throw new NotImplementedException();
    }
}