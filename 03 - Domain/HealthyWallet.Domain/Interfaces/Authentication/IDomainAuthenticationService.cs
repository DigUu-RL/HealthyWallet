using HealthyWallet.Domain.Exceptions.Abstractions;
using HealthyWallet.Domain.Exceptions.Abstractions.Authentication;
using HealthyWallet.Domain.Models.Authentication;
using HealthyWallet.Domain.Requests.Authentication;
using HealthyWallet.Infrastructure.Data.Entities.Identity;

namespace HealthyWallet.Domain.Interfaces.Authentication;

/// <summary>
/// Provides authentication operations such as sign-in, token validation,
/// and refresh token management.
/// </summary>
public interface IDomainAuthenticationService
{
    /// <summary>
    /// Authenticates a user by validating their credentials and generates a new access token.
    /// </summary>
    /// <param name="model">The sign-in request containing the username and password.</param>
    /// <returns>
    /// An <see cref="AccessTokenModel"/> representing the generated access token.
    /// </returns>
    /// <exception cref="NotFoundException">Thrown when the user is not found.</exception>
    /// <exception cref="InvalidCredentialsException">Thrown when the provided password is invalid.</exception>
    Task<AccessTokenModel> SignInAsync(SignInRequest model);

    /// <summary>
    /// Validates the provided token and returns the corresponding user if valid.
    /// </summary>
    /// <param name="type">The type of the token (e.g., "Bearer").</param>
    /// <param name="token">The token string to validate.</param>
    /// <returns>
    /// A <see cref="User"/> associated with the validated token.
    /// </returns>
    /// <exception cref="InvalidTokenException">Thrown if the token type is invalid or the token cannot be validated.</exception>
    Task<User> ValidateTokenAsync(string type, string token);

    /// <summary>
    /// Creates and stores a new refresh token for the specified user.
    /// </summary>
    /// <param name="request">The request containing the user reference and expiration date.</param>
    Task CreateRefreshTokenAsync(RefreshTokenRequest request);

    /// <summary>
    /// Retrieves an existing refresh token for the specified user.
    /// </summary>
    /// <param name="userReferenceId">The unique identifier of the user.</param>
    /// <returns>
    /// A <see cref="RefreshTokenModel"/> if found.
    /// </returns>
    /// <exception cref="InvalidCredentialsException">Thrown when no refresh token is found for the user.</exception>
    Task<RefreshTokenModel> GetRefreshTokenAsync(Guid userReferenceId);
}