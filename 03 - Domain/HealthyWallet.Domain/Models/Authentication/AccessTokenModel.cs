namespace HealthyWallet.Domain.Models.Authentication;

public record AccessTokenModel(string Token, DateTime ExpiresIn);