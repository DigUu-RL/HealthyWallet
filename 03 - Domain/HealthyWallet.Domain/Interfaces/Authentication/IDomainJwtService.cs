

using HealthyWallet.Domain.Models.Authentication;
using HealthyWallet.Infrastructure.Data.Entities.Identity;

namespace HealthyWallet.Domain.Interfaces.Authentication;

/// <summary>
/// Interface for managing JWT token operations, including generation, validation, 
/// and refresh token storage and retrieval.
/// </summary>
public interface IDomainJwtService
{
    /// <summary>
    /// Generates a new access token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the token will be generated.</param>
    /// <returns>An <see cref="AccessTokenModel"/> representing the generated token.</returns>
    AccessTokenModel GenerateToken(User user);

    /// <summary>
    /// Validates the specified JWT token asynchronously.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>A <see cref="User"/> if the token is valid; otherwise, throws an exception.</returns>
    Task<User> ValidateTokenAsync(string token);

    /// <summary>
    /// Stores the provided refresh token asynchronously.
    /// </summary>
    /// <param name="refreshToken">The refresh token to store.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task StoreRefreshTokenAsync(RefreshTokenModel refreshToken);

    /// <summary>
    /// Retrieves the refresh token associated with the specified user asynchronously.
    /// </summary>
    /// <param name="userReferenceId">The unique identifier of the user.</param>
    /// <returns>
    /// A <see cref="RefreshTokenModel"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<RefreshTokenModel?> GetRefreshTokenAsync(Guid userReferenceId);
}
