using System.Linq.Expressions;

namespace HealthyWallet.Infrastructure.Repository.DesignPattern.Specification.Abstractions;

public sealed class NotSpecification<T>(Specification<T> source) : Specification<T>
{
    private Specification<T> Source { get; } = source ?? throw new ArgumentNullException(nameof(source));

    public override Expression<Func<T, bool>> ToExpression()
    {
        Expression<Func<T, bool>> expression = Source.ToExpression();
        return Expression.Lambda<Func<T, bool>>(Expression.Not(expression.Body), expression.Parameters);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is NotSpecification<T> other) return Equals(other);

        return false;
    }

    private bool Equals(NotSpecification<T> other) => Source.Equals(other.Source);
    public override int GetHashCode() => Source.GetHashCode();
}