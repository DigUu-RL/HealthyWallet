using HealthyWallet.Domain.Models.Authentication;

namespace HealthyWallet.Application.DTOs.Authentication;

public record AccessTokenDto(string Token, DateTime ExpiresIn)
{
    public AccessTokenDto(AccessTokenModel accessToken) : this(accessToken.Token, accessToken.ExpiresIn) {}
}
