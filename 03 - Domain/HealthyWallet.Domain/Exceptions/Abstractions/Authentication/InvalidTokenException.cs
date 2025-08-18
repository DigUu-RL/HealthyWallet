using System.Net;

namespace HealthyWallet.Domain.Exceptions.Abstractions.Authentication;

public class InvalidTokenException(string message) : HealthyWalletException(message, HttpStatusCode.Unauthorized);