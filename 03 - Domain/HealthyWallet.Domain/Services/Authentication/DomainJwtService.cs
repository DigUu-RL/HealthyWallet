using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HealthyWallet.Domain.Exceptions.Abstractions;
using HealthyWallet.Domain.Exceptions.Abstractions.Authentication;
using HealthyWallet.Domain.Helpers.Authentication;
using HealthyWallet.Domain.Interfaces.Authentication;
using HealthyWallet.Domain.Models.Authentication;
using HealthyWallet.Infrastructure.Data.Entities.Identity;
using HealthyWallet.Infrastructure.Data.Enums.Identity;
using HealthyWallet.Infrastructure.Repository.DesignPattern.Specification;
using HealthyWallet.Infrastructure.Repository.DesignPattern.Specification.Contracts.Identity;
using HealthyWallet.Infrastructure.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HealthyWallet.Domain.Services.Authentication;

public class DomainJwtService(
    IConfiguration configuration,
    TokenValidationParameters parameters,
    IReadOnlyRepository<User> userRepository
) : IDomainJwtService
{
    private readonly JwtSecurityTokenHandler _handler = new();

    public AccessTokenModel GenerateToken(User user)
    {
        IConfigurationSection jwtSection = configuration.GetSection("Jwt");

        var secretKey = jwtSection.GetValue<string>("SecretKey") ??
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

    public async Task<User> ValidateToken(string token)
    {
        TokenValidationResult result = await _handler.ValidateTokenAsync(token, parameters);
        if (!result.IsValid) throw new InvalidTokenException("Invalid token");

        result.Claims.TryGetValue(HealthyWalletJwtClaimNames.Id, out object? id);
        result.Claims.TryGetValue(HealthyWalletJwtClaimNames.ReferenceId, out object? referenceId);
        result.Claims.TryGetValue(HealthyWalletJwtClaimNames.Name, out object? name);
        result.Claims.TryGetValue(HealthyWalletJwtClaimNames.Username, out object? username);
        result.Claims.TryGetValue(HealthyWalletJwtClaimNames.Email, out object? email);
        result.Claims.TryGetValue(HealthyWalletJwtClaimNames.Type, out object? type);

        bool exists = id is not null && referenceId is not null && name is not null && username is not null &&
                      email is not null;
        if (!exists) throw new InvalidTokenException("Invalid token and claims");

        long parsedId = long.Parse(id!.ToString()!);
        Guid parsedReferenceId = Guid.Parse(referenceId!.ToString()!);
        string parsedName = name!.ToString()!;
        string parsedUsername = username!.ToString()!;
        string parsedEmail = email!.ToString()!;
        UserType parsedType = Enum.Parse<UserType>(type!.ToString()!);

        return new User
        {
            Id = parsedId,
            ReferenceId = parsedReferenceId,
            Name = parsedName,
            UserName = parsedUsername,
            Email = parsedEmail,
            Type = parsedType,
            PasswordHash = string.Empty
        };
    }
}