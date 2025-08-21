using HealthyWallet.Infrastructure.Data.Entities.Identity;
using HealthyWallet.Infrastructure.Data.Enums.Identity;

namespace HealthyWallet.Application.DTOs.Authentication;

public record AuthUserDto(long Id, Guid ReferenceId, string Name, string UserName, string Email, UserType Type)
{
    public AuthUserDto(User user) : this(
        user.Id,
        user.ReferenceId,
        user.Name,
        user.UserName,
        user.Email,
        user.Type
    )
    {
        
    }
}