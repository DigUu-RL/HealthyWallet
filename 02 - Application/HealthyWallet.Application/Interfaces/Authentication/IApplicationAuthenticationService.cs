using HealthyWallet.Application.DTOs.Authentication;
using HealthyWallet.Domain.Exceptions.Abstractions.Authentication;
using HealthyWallet.Domain.Requests.Authentication;
using HealthyWallet.Infrastructure.Data.Entities.Identity;

namespace HealthyWallet.Application.Interfaces.Authentication;

/// <summary>
/// Provides application-level authentication operations such as sign-in,
/// token validation, and refresh token management.
/// </summary>
public interface IApplicationAuthenticationService
{
    /// <summary>
    /// Authenticates a user by validating their credentials and generates a new access token DTO.
    /// </summary>
    /// <param name="request">The sign-in request containing the username and password.</param>
    /// <returns>
    /// An <see cref="AccessTokenDto"/> representing the generated access token.
    /// </returns>
    /// <exception cref="InvalidCredentialsException">Thrown when the username or password is invalid.</exception>
    Task<AccessTokenDto> SignInAsync(SignInRequest request);

    /// <summary>
    /// Validates the provided token and returns the corresponding user if valid.
    /// </summary>
    /// <param name="token">The token string to validate (must include type and value, e.g., "Bearer &lt;token&gt;").</param>
    /// <returns>
    /// A <see cref="User"/> associated with the validated token.
    /// </returns>
    /// <exception cref="InvalidTokenException">Thrown when the token is null, empty, or invalid.</exception>
    Task<User> ValidateTokenAsync(string? token);

    /// <summary>
    /// Creates and stores a new refresh token for the specified user, returning a DTO representation.
    /// </summary>
    /// <param name="request">The request containing the user reference and expiration date.</param>
    /// <exception cref="InvalidCredentialsException">Thrown when the user reference ID is invalid.</exception>
    Task CreateRefreshTokenAsync(RefreshTokenRequest request);

    /// <summary>
    /// Retrieves an existing refresh token for the specified user, returning a DTO representation.
    /// </summary>
    /// <param name="userReferenceId">The unique identifier of the user.</param>
    /// <returns>
    /// A <see cref="RefreshTokenDto"/> if found.
    /// </returns>
    Task<RefreshTokenDto> GetRefreshTokenAsync(Guid userReferenceId);
}