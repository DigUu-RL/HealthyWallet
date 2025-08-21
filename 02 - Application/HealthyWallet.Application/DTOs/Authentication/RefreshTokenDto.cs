using HealthyWallet.Domain.Models.Authentication;

namespace HealthyWallet.Application.DTOs.Authentication;

public class RefreshTokenDto(Guid Token, Guid UserReferenceId, DateTime ExpiresIn)
{
    public RefreshTokenDto(RefreshTokenModel refreshToken) : this(
        refreshToken.Token,
        refreshToken.UserReferenceId,
        refreshToken.ExpiresIn
    )
    {
    }
}