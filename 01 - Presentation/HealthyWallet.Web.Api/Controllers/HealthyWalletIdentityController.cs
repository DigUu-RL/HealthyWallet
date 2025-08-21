using HealthyWallet.Infrastructure.Data.Entities.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthyWallet.Web.Api.Controllers;

public class HealthyWalletIdentityController : Controller
{
    public new User User => (HttpContext.User.Identity ?? HttpContext.Items[nameof(User)]) as User ??
                            throw new UnauthorizedAccessException();
}