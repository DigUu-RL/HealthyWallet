namespace HealthyWallet.Domain.Requests.Authentication;

public record RefreshTokenRequest(Guid UserReferenceId, DateTime ExpiresIn);