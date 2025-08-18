using System.IdentityModel.Tokens.Jwt;
using System.Net;
using HealthyWallet.Domain.Interfaces.Authentication;
using HealthyWallet.Domain.Models;
using HealthyWallet.Infrastructure.CrossCutting.Extensions.Enum;
using HealthyWallet.Infrastructure.CrossCutting.Helpers;
using HealthyWallet.Infrastructure.Data.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using JSON = NetJSON.NetJSON;

namespace HealthyWallet.Infrastructure.CrossCutting.Middlewares;

public class JwtMiddleware(TokenValidationParameters parameters, IDomainJwtService jwtService) : IMiddleware
{
    private const HttpStatusCode Unauthorized = HttpStatusCode.Unauthorized;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string? authorization = context.Request.Headers[nameof(Authorization)].FirstOrDefault();

        if (string.IsNullOrEmpty(authorization))
        {
            await WriteUnauthorized(context, "Missing Authorization header");
            return;
        }

        string[] parts = authorization.Split(' ');

        if (parts.Length != 2 || !parts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
        {
            await WriteUnauthorized(context, "Invalid Authorization header format");
            return;
        }

        string token = parts[1];
        User user = await jwtService.ValidateToken(token);

        AttachUser(context, user);
        await next.Invoke(context);
    }

    private static async Task WriteUnauthorized(HttpContext context, string message)
    {
        string json = JSON.Serialize(new ExceptionModel(
            Unauthorized.ToCapitalizedString(),
            (int) Unauthorized,
            message
        ));

        context.Response.StatusCode = (int) Unauthorized;
        context.Response.ContentType = ContentTypes.Json;
        
        await context.Response.WriteAsync(json);
    }

    private void AttachUser(HttpContext context, User user)
    {
        context.Items["User"] = user;
    }
}
