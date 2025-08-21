using HealthyWallet.Application.DTOs.Authentication;
using HealthyWallet.Application.Interfaces.Authentication;
using HealthyWallet.Domain.Exceptions.Abstractions.Authentication;
using HealthyWallet.Domain.Interfaces.Authentication;
using HealthyWallet.Domain.Models.Authentication;
using HealthyWallet.Domain.Requests.Authentication;
using HealthyWallet.Infrastructure.Data.Entities.Identity;

namespace HealthyWallet.Application.Services.Authentication;

/// <summary>
/// Implementation of <see cref="IApplicationAuthenticationService"/> that provides
/// application-level authentication using domain authentication services.
/// </summary>
public class ApplicationAuthenticationService(
    IDomainAuthenticationService authenticationService
) : IApplicationAuthenticationService
{
    /// <inheritdoc cref="IApplicationAuthenticationService.SignInAsync"/>
    public async Task<AccessTokenDto> SignInAsync(SignInRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidCredentialsException("Invalid username or password");

        AccessTokenModel model = await authenticationService.SignInAsync(request);
        return new AccessTokenDto(model);
    }

    /// <inheritdoc cref="IApplicationAuthenticationService.ValidateTokenAsync"/>
    public async Task<User> ValidateTokenAsync(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) throw new InvalidTokenException("Invalid token");

        string[] parts = token.Split(' ');

        if (parts.Length != 2) throw new InvalidTokenException("Invalid token");
        if (!parts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase)) throw new InvalidTokenException("Invalid token type");

        User user = await authenticationService.ValidateTokenAsync(parts[0], parts[1]);
        return user;
    }

    /// <inheritdoc cref="IApplicationAuthenticationService.CreateRefreshTokenAsync"/>
    public async Task CreateRefreshTokenAsync(RefreshTokenRequest request)
    {
        if (request.UserReferenceId == Guid.Empty) throw new InvalidCredentialsException("Invalid user reference id");
        await authenticationService.CreateRefreshTokenAsync(request);
    }

    /// <inheritdoc cref="IApplicationAuthenticationService.GetRefreshTokenAsync"/>
    public async Task<RefreshTokenDto> GetRefreshTokenAsync(Guid userReferenceId)
    {
        RefreshTokenModel refreshToken = await authenticationService.GetRefreshTokenAsync(userReferenceId);
        return new RefreshTokenDto(refreshToken);
    }
}