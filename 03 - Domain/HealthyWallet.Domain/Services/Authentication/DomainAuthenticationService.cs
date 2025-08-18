using HealthyWallet.Domain.Exceptions.Abstractions;
using HealthyWallet.Domain.Exceptions.Abstractions.Authentication;
using HealthyWallet.Domain.Helpers;
using HealthyWallet.Domain.Interfaces.Authentication;
using HealthyWallet.Domain.Models.Authentication;
using HealthyWallet.Domain.Requests.Authentication;
using HealthyWallet.Infrastructure.Data.Entities.Identity;
using HealthyWallet.Infrastructure.Repository.DesignPattern.Specification.Contracts.Identity;
using HealthyWallet.Infrastructure.Repository.Interfaces;

namespace HealthyWallet.Domain.Services.Authentication;

public class DomainAuthenticationService(
    IDomainJwtService jwtService,
    IReadOnlyRepository<User> userRepository
) : IDomainAuthenticationService
{
    public async Task<AccessTokenModel> SignIn(SignInRequest model)
    {
        User? user = await userRepository.GetByAsync(
            UserSpecification.ByEmail(model.Username) ||
            UserSpecification.ByUserName(model.Username)
        );

        if (user is null) throw new NotFoundException("User not found");
        if (!PasswordHelper.Verify(model.Password, user.PasswordHash)) throw new InvalidCredentialsException("Password not matches");

        AccessTokenModel token = jwtService.GenerateToken(user);
        return token;
    }

    public async Task<User> ValidateToken(string type, string token)
    {
        if (!type.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
            throw new InvalidTokenException("Invalid token type");

        User user = await jwtService.ValidateToken(token);
        return user;
    }
}