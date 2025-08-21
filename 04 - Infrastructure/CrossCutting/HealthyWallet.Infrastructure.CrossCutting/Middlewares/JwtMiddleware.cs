using System.Net;
using System.Security.Claims;
using HealthyWallet.Domain.Helpers;
using HealthyWallet.Domain.Interfaces.Authentication;
using HealthyWallet.Domain.Models;
using HealthyWallet.Infrastructure.CrossCutting.Extensions.Enum;
using HealthyWallet.Infrastructure.CrossCutting.Helpers;
using HealthyWallet.Infrastructure.Data.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace HealthyWallet.Infrastructure.CrossCutting.Middlewares;

public class JwtMiddleware(IDomainJwtService jwtService) : IMiddleware
{
    private const HttpStatusCode Unauthorized = HttpStatusCode.Unauthorized;
    private const string AuthenticationType = "HealthyWalletJwtAuthentication"; 

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        StringValues authorization = context.Request.Headers[nameof(Authorization)];

        if (string.IsNullOrEmpty(authorization))
        {
            await WriteUnauthorized(context, $"Missing {nameof(Authorization)} header");
            return;
        }

        string[] parts = authorization.ToString().Split(' ');

        if (parts.Length != 2 || !parts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
        {
            await WriteUnauthorized(context, $"Invalid {nameof(Authorization)} header format");
            return;
        }

        string token = parts[1];
        
        User user = await jwtService.ValidateTokenAsync(token);
        user.AuthenticationType = AuthenticationType;
        user.IsAuthenticated = true;

        AttachUser(context, user);
        await next.Invoke(context);
    }

    private static async Task WriteUnauthorized(HttpContext context, string message)
    {
        string json = Json.Serialize(new ExceptionModel(
            Unauthorized.ToCapitalizedString(),
            (int) Unauthorized,
            message
        ));

        context.Response.StatusCode = (int) Unauthorized;
        context.Response.ContentType = ContentTypes.Json;

        await context.Response.WriteAsync(json);
    }

    private static void AttachUser(HttpContext context, User user)
    {
        var principal = new ClaimsPrincipal(user);

        Thread.CurrentPrincipal = principal;
        
        context.User = principal;
        context.Items[nameof(User)] = user;
    }
}