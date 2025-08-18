using System.Net;

namespace HealthyWallet.Domain.Exceptions.Abstractions.Authentication;

public class InvalidCredentialsException(string message) : HealthyWalletException(message, HttpStatusCode.Unauthorized);