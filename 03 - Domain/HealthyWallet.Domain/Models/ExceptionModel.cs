namespace HealthyWallet.Domain.Models;

public record ExceptionModel(string Error, int Code, string Message, string? Inner = null);