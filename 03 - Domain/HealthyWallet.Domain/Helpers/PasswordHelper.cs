using Hasher = BCrypt.Net.BCrypt;

namespace HealthyWallet.Domain.Helpers;

/// <summary>
/// Provides helper methods for hashing and verifying passwords using the BCrypt algorithm.
/// </summary>
public static class PasswordHelper
{
    private const int WorkFactor = 12;

    /// <summary>
    /// Hashes the given plain-text password using the BCrypt algorithm with a predefined work factor.
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <returns>A hashed password string including the salt.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the password is null or whitespace.</exception>
    public static string Hash(string password)
    {
        return string.IsNullOrWhiteSpace(password)
            ? throw new ArgumentNullException(nameof(password))
            : Hasher.HashPassword(password, WorkFactor);
    }

    /// <summary>
    /// Verifies whether the provided plain-text password matches the hashed password.
    /// </summary>
    /// <param name="password">The plain-text password to verify.</param>
    /// <param name="hashedPassword">The previously hashed password to compare against.</param>
    /// <returns><c>true</c> if the password matches the hash; otherwise, <c>false</c>.</returns>
    public static bool Verify(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword)) return false;
        return Hasher.Verify(password, hashedPassword);
    }
}