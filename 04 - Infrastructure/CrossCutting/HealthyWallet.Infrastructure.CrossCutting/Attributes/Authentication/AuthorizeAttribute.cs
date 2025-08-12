using System.Net;
using HealthyWallet.Domain.Models;
using HealthyWallet.Infrastructure.CrossCutting.Extensions;
using HealthyWallet.Infrastructure.CrossCutting.Extensions.Enum;
using HealthyWallet.Infrastructure.Data.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HealthyWallet.Infrastructure.CrossCutting.Attributes.Authentication;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        bool isAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (isAnonymous) return;

        if (context.HttpContext.Items[nameof(User)] is User) return;
        
        const HttpStatusCode code = HttpStatusCode.Unauthorized;

        context.Result = new JsonResult(
            new ExceptionModel(
                code.ToCapitalizedString(),
                code.GetHashCode(),
                "Unable to access this content. Please, sign-in in the Healthy Wallet app"
            )
        );
    }
}