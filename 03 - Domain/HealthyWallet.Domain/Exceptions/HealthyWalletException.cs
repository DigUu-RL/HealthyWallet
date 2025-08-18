using System.Net;

namespace HealthyWallet.Domain.Exceptions;

public abstract class HealthyWalletException : Exception
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;

    protected HealthyWalletException(string message) : base(message)
    {
    }

    protected HealthyWalletException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}