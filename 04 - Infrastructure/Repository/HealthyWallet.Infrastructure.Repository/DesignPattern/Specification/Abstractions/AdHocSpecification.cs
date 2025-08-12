using System.Linq.Expressions;

namespace HealthyWallet.Infrastructure.Repository.DesignPattern.Specification.Abstractions;

public sealed class AdHocSpecification<T>(Expression<Func<T, bool>> expression) : Specification<T>
{
    private readonly Lazy<string> _expressionString = new(expression.ToString);
    private readonly Expression<Func<T, bool>> _expression = expression ?? throw new ArgumentNullException(nameof(expression));

    public override Expression<Func<T, bool>> ToExpression() => _expression;
    
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is AdHocSpecification<T> other) return Equals(other);
        
        return false;
    }

    private bool Equals(AdHocSpecification<T> other) => _expressionString.Equals(other._expressionString) && _expression.Equals(other._expression);
    public override int GetHashCode() => HashCode.Combine(_expressionString, _expression);
}