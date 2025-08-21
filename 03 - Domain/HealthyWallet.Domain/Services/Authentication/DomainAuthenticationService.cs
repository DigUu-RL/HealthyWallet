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

/// <summary>
/// Implementation of <see cref="IDomainAuthenticationService"/> that manages
/// user authentication, token validation, and refresh token handling.
/// </summary>
public class DomainAuthenticationService(
    IDomainJwtService jwtService,
    IReadOnlyRepository<User> userRepository
) : IDomainAuthenticationService
{
    /// <inheritdoc cref="IDomainAuthenticationService.SignInAsync"/>
    public async Task<AccessTokenModel> SignInAsync(SignInRequest model)
    {
        userRepository.With(user => user.Claims);
        
        User? user = await userRepository.GetByAsync(
            UserSpecification.ByEmail(model.Username) ||
            UserSpecification.ByUserName(model.Username)
        );

        if (user is null) throw new NotFoundException("User not found");
        if (!PasswordHelper.Verify(model.Password, user.PasswordHash)) throw new InvalidCredentialsException("Password not matches");

        AccessTokenModel token = jwtService.GenerateToken(user);
        return token;
    }

    /// <inheritdoc cref="IDomainAuthenticationService.ValidateTokenAsync"/>
    public async Task<User> ValidateTokenAsync(string type, string token)
    {
        if (!type.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
            throw new InvalidTokenException("Invalid token type");

        User user = await jwtService.ValidateTokenAsync(token);
        return user;
    }

    /// <inheritdoc cref="IDomainAuthenticationService.CreateRefreshTokenAsync"/>
    public async Task CreateRefreshTokenAsync(RefreshTokenRequest request)
    {
        var refreshToken = new RefreshTokenModel(
            Guid.CreateVersion7(),
            request.UserReferenceId,
            request.ExpiresIn
        );

        await jwtService.StoreRefreshTokenAsync(refreshToken);
    }

    /// <inheritdoc cref="IDomainAuthenticationService.GetRefreshTokenAsync"/>
    public async Task<RefreshTokenModel> GetRefreshTokenAsync(Guid userReferenceId)
    {
        RefreshTokenModel refreshToken = await jwtService.GetRefreshTokenAsync(userReferenceId) ??
                                         throw new InvalidCredentialsException("Refresh token not found");

        return refreshToken;
    }
}