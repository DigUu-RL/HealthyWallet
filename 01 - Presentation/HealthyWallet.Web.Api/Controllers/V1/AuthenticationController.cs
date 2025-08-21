using System.Net;
using Asp.Versioning;
using HealthyWallet.Application.DTOs.Authentication;
using HealthyWallet.Application.Interfaces.Authentication;
using HealthyWallet.Domain.Requests.Authentication;
using HealthyWallet.Infrastructure.Data.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using AuthorizeAttribute = HealthyWallet.Infrastructure.CrossCutting.Attributes.Authentication.AuthorizeAttribute;

namespace HealthyWallet.Web.Api.Controllers.V1;

[ApiController]
[ApiVersion(1.0)]
[AllowAnonymous]
[Route("v1/[controller]")]
public class AuthenticationController(IApplicationAuthenticationService authenticationService) : HealthyWalletIdentityController
{
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest model)
    {
        AccessTokenDto accessToken = await authenticationService.SignInAsync(model);
        return Ok(accessToken);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        StringValues authorization = HttpContext.Request.Headers[nameof(Authorization)];
        User user = await authenticationService.ValidateTokenAsync(authorization);

        return Ok(new AuthUserDto(user));
    }

    [Authorize]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        StringValues authorization = HttpContext.Request.Headers[nameof(Authorization)];
        User user = await authenticationService.ValidateTokenAsync(authorization);

        await authenticationService.CreateRefreshTokenAsync(
            new RefreshTokenRequest(
                user.ReferenceId,
                DateTime.UtcNow.AddDays(1)
            )
        );

        return Ok();
    }
}