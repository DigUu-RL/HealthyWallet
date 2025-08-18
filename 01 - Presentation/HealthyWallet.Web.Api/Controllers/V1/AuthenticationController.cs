using Asp.Versioning;
using HealthyWallet.Application.DTOs;
using HealthyWallet.Application.Interfaces.Authentication;
using HealthyWallet.Domain.Requests.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthorizeAttribute = HealthyWallet.Infrastructure.CrossCutting.Attributes.Authentication.AuthorizeAttribute;

namespace HealthyWallet.Web.Api.Controllers.V1;

[ApiController]
[ApiVersion(1.0)]
[AllowAnonymous]
[Route("[controller]")]
public class AuthenticationController(IApplicationAuthenticationService authenticationService) : Controller
{
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest model)
    {
        AccessTokenDto accessToken = await authenticationService.SignIn(model);
        return Ok(accessToken);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        return Ok();
    }
}