using System.IdentityModel.Tokens.Jwt;
using HealthyWallet.Domain.Interfaces.Authentication;
using HealthyWallet.Domain.Models.Authentication;
using HealthyWallet.Domain.Requests.Authentication;
using HealthyWallet.Infrastructure.Repository.Interfaces.Identity;
using Microsoft.Extensions.Configuration;

namespace HealthyWallet.Domain.Services.Authentication;

public class DomainAuthenticationService(
    IConfiguration _configuration, 
    IUserRepository userRepository
) : IDomainAuthenticationService
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    
    public Task<AccessTokenModel> SignIn(SignInRequest model)
    {
        throw new NotImplementedException();
    }

    public Task ValidateToken(string type, string token)
    {
        throw new NotImplementedException();
    }
}