using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HealthyWallet.Domain.Exceptions.Abstractions;
using HealthyWallet.Domain.Exceptions.Abstractions.Authentication;
using HealthyWallet.Domain.Helpers;
using HealthyWallet.Domain.Helpers.Authentication;
using HealthyWallet.Domain.Interfaces.Authentication;
using HealthyWallet.Domain.Models.Authentication;
using HealthyWallet.Infrastructure.Data.Entities.Identity;
using HealthyWallet.Infrastructure.Data.Enums.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace HealthyWallet.Domain.Services.Authentication;

/// <summary>
/// Implementation of <see cref="IDomainJwtService"/> that handles JWT generation,
/// validation, and refresh token storage using Redis.
/// </summary>
public class DomainJwtService(
    IConfiguration configuration,
    TokenValidationParameters parameters,
    IDatabase redisDatabase,
    IHostEnvironment env
) : IDomainJwtService
{
    private readonly JwtSecurityTokenHandler _handler = new();

    /// <inheritdoc cref="IDomainJwtService.GenerateToken"/>
    public AccessTokenModel GenerateToken(User user)
    {
        IConfigurationSection jwtSection = configuration.GetSection("Jwt");
        
        string secretKey = jwtSection.GetValue<string>("SecretKey") ?? 
                           throw new InvalidOperationException("Secret key not found");
        
        byte[] key = Encoding.UTF8.GetBytes(secretKey);

        var claims = new List<Claim>
        {
            new(HealthyWalletJwtClaimNames.Id, $"{user.Id}"),
            new(HealthyWalletJwtClaimNames.ReferenceId, $"{user.ReferenceId}"),
            new(HealthyWalletJwtClaimNames.Name, user.Name),
            new(HealthyWalletJwtClaimNames.Username, user.UserName),
            new(HealthyWalletJwtClaimNames.Email, user.Email),
            new(HealthyWalletJwtClaimNames.Type, $"{user.Type}")
        };

        IEnumerable<Claim> userClaims = user.Claims.Select(claim => (Claim) claim);
        claims.AddRange(userClaims);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = parameters.ValidIssuer,
            Audience = parameters.ValidAudience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            NotBefore = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        JwtSecurityToken security = _handler.CreateJwtSecurityToken(descriptor);
        string token = _handler.WriteToken(security);

        return new AccessTokenModel(token, descriptor.Expires.GetValueOrDefault());
    }

    /// <inheritdoc cref="IDomainJwtService.ValidateTokenAsync"/>
    public async Task<User> ValidateTokenAsync(string token)
    {
        TokenValidationResult result = await _handler.ValidateTokenAsync(token, parameters);
        if (!result.IsValid) throw new InvalidTokenException("Invalid token");

        (long id, Guid referenceId, string name, string username, string email, UserType type) = ValidateClaims(result.Claims);
        
        return new User
        {
            Id = id,
            ReferenceId = referenceId,
            Name = name,
            UserName = username,
            Email = email,
            Type = type,
            PasswordHash = string.Empty
        };
    }

    /// <summary>
    /// Validates the required claims extracted from the JWT token.
    /// </summary>
    /// <param name="claims">The dictionary of token claims.</param>
    /// <returns>A tuple containing the parsed claim values.</returns>
    /// <exception cref="InvalidTokenException">Thrown if any required claim is missing or invalid.</exception>
    private static (long id, Guid referenceId, string name, string username, string email, UserType type)
        ValidateClaims(IDictionary<string, object> claims)
    {
        claims.TryGetValue(HealthyWalletJwtClaimNames.Id, out object? id);
        claims.TryGetValue(HealthyWalletJwtClaimNames.ReferenceId, out object? referenceId);
        claims.TryGetValue(HealthyWalletJwtClaimNames.Name, out object? name);
        claims.TryGetValue(HealthyWalletJwtClaimNames.Username, out object? username);
        claims.TryGetValue(HealthyWalletJwtClaimNames.Email, out object? email);
        claims.TryGetValue(HealthyWalletJwtClaimNames.Type, out object? type);

        bool exists = id is not null &&
                      referenceId is not null &&
                      name is not null &&
                      username is not null &&
                      email is not null &&
                      type is not null;

        if (!exists) throw new InvalidTokenException("Invalid token and claims");

        long parsedId = long.Parse(id!.ToString()!);
        Guid parsedReferenceId = Guid.Parse(referenceId!.ToString()!);
        string parsedName = name!.ToString()!;
        string parsedUsername = username!.ToString()!;
        string parsedEmail = email!.ToString()!;
        UserType parsedType = Enum.Parse<UserType>(type!.ToString()!);

        return (parsedId, parsedReferenceId, parsedName, parsedUsername, parsedEmail, parsedType);
    }

    /// <inheritdoc cref="IDomainJwtService.StoreRefreshTokenAsync"/>
    public async Task StoreRefreshTokenAsync(RefreshTokenModel refreshToken)
    {
        string json = Json.Serialize(refreshToken);
        TimeSpan expiresIn = refreshToken.ExpiresIn - DateTime.UtcNow;

        var key = new RedisKey(GetRefreshTokenKey(refreshToken.UserReferenceId));
        var value = new RedisValue(json);

        await redisDatabase.StringSetAsync(key, value, expiresIn);
    }

    /// <inheritdoc cref="IDomainJwtService.GetRefreshTokenAsync"/>
    public async Task<RefreshTokenModel?> GetRefreshTokenAsync(Guid userReferenceId)
    {
        string? json = await redisDatabase.StringGetAsync(GetRefreshTokenKey(userReferenceId));

        return json is not null
            ? Json.Deserialize<RefreshTokenModel>(json)
            : null;
    }

    /// <summary>
    /// Builds the Redis key for storing or retrieving a refresh token.
    /// </summary>
    /// <param name="userReferenceId">The user's unique reference ID.</param>
    /// <returns>The Redis key string.</returns>
    private string GetRefreshTokenKey(Guid userReferenceId)
    {
        IConfigurationSection section = configuration.GetSection("Redis");

        string key = section.GetValue<string>("Key") ??
                     throw new EnvironmentKeyNotFoundException("Could not find redis key");

        return string.Format(key, env.EnvironmentName, userReferenceId);
    }
}