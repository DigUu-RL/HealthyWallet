using System.Net;
using HealthyWallet.Domain.Models;
using HealthyWallet.Infrastructure.CrossCutting.Exceptions;
using HealthyWallet.Infrastructure.CrossCutting.Extensions;
using HealthyWallet.Infrastructure.CrossCutting.Helpers;
using Microsoft.AspNetCore.Http;
using JSON = NetJSON.NetJSON;

namespace HealthyWallet.Infrastructure.CrossCutting.Middlewares;

public class GlobalExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (HealthyWalletException ex)
        {
            await HandleExceptionAsync(context, ex, ex.StatusCode);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError
    )
    {
        if (exception is HealthyWalletException walletException) statusCode = walletException.StatusCode;

        int code = Convert.ToInt32(statusCode);

        string json = JSON.Serialize(new ExceptionModel(
            statusCode.ToCapitalizedString(),
            code,
            exception.Message,
            exception.InnerException?.Message
        ));

        context.Response.StatusCode = code;
        context.Response.ContentType = ContentType.Json;
        
        await context.Response.WriteAsync(json);
    }
}