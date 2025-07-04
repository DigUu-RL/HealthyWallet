using HealthyWallet.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace HealthyWallet.Infrastructure.CrossCutting.Middlewares;

public class DbTransactionMiddleware(HealthyWalletContext dbContext) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string method = context.Request.Method;

        if (HttpMethods.IsGet(method) || HttpMethods.IsOptions(method))
        {
            await next.Invoke(context);
            return;
        }

        await using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await next.Invoke(context);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}