using System.Net;

namespace HealthyWallet.Domain.Exceptions.Abstractions;

public class NotFoundException(string message) : HealthyWalletException(message, HttpStatusCode.NotFound);