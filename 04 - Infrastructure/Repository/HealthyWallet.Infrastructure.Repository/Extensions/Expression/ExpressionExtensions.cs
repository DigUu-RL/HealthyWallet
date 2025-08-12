using System.Linq.Expressions;

namespace HealthyWallet.Infrastructure.Repository.Extensions.Expression;

public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> AndAlso<T>(
        this Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second
    )
    {
        ArgumentNullException.ThrowIfNull(first, nameof(first));
        ArgumentNullException.ThrowIfNull(second, nameof(second));
        
        return first.Compose(second, System.Linq.Expressions.Expression.AndAlso);
    }

    public static Expression<Func<T, bool>> OrElse<T>(
        this Expression<Func<T, bool>> first, 
        Expression<Func<T, bool>> second
    )
    {
        ArgumentNullException.ThrowIfNull(first, nameof(first));
        ArgumentNullException.ThrowIfNull(second, nameof(second));
        
        return first.Compose(second, System.Linq.Expressions.Expression.OrElse);
    }

    private static Expression<T> Compose<T>(
        this Expression<T> first, Expression<T> second, 
        Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression, System.Linq.Expressions.Expression> merge
    )
    {
        ArgumentNullException.ThrowIfNull(first, nameof(first));
        ArgumentNullException.ThrowIfNull(second, nameof(second));
        
        Dictionary<ParameterExpression, ParameterExpression> map = first.Parameters.Select((parameter, index) => new 
        {
            FirstParameter = parameter, 
            SecondParamter = second.Parameters[index]
        }).ToDictionary(
            parameter => parameter.SecondParamter, 
            parameter => parameter.FirstParameter
        );
        
        System.Linq.Expressions.Expression secondBody = ExpressionParameterRebinder.ReplaceParameters(map, second.Body);
        return System.Linq.Expressions.Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }

    private class ExpressionParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map) : ExpressionVisitor
    {
        public static System.Linq.Expressions.Expression ReplaceParameters(
            Dictionary<ParameterExpression, ParameterExpression> map, 
            System.Linq.Expressions.Expression exp
        )
        {
            return new ExpressionParameterRebinder(map).Visit(exp);
        }

        protected override System.Linq.Expressions.Expression VisitParameter(ParameterExpression parameter)
        { 
            if (map.TryGetValue(parameter, out ParameterExpression? result)) parameter = result;
            return base.VisitParameter(parameter);
        }
    }
}