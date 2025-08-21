using System.Net;

namespace HealthyWallet.Domain.Exceptions.Abstractions;

public class EnvironmentKeyNotFoundException(string message) : HealthyWalletException(message, HttpStatusCode.UnprocessableEntity);