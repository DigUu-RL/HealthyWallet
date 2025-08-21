namespace HealthyWallet.Domain.Models.Authentication;

public record RefreshTokenModel(Guid Token, Guid UserReferenceId, DateTime ExpiresIn);
