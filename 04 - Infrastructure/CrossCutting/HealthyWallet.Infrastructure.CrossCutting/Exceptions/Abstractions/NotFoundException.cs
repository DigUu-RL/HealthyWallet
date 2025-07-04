using System.Net;

namespace HealthyWallet.Infrastructure.CrossCutting.Exceptions.Abstractions;

public class NotFoundException(string message) : HealthyWalletException(message, HttpStatusCode.NotFound);