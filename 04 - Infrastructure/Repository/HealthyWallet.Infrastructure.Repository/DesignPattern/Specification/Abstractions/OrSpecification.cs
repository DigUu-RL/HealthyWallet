using System.Linq.Expressions;
using HealthyWallet.Infrastructure.Repository.Extensions.Expression;

namespace HealthyWallet.Infrastructure.Repository.DesignPattern.Specification.Abstractions;

public sealed class OrSpecification<T>(Specification<T> left, Specification<T> right) : Specification<T>
{
    private Specification<T> Left { get; } = left ?? throw new ArgumentNullException(nameof(left));
    private Specification<T> Right { get; } = right ?? throw new ArgumentNullException(nameof(right));

    public override Expression<Func<T, bool>> ToExpression()
    {
        Expression<Func<T, bool>> first = Left.ToExpression();
        Expression<Func<T, bool>> second = Right.ToExpression();

        return first.OrElse(second);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is OrSpecification<T> other) return Equals(other);

        return false;
    }

    private bool Equals(OrSpecification<T> other) => Left.Equals(other.Left) && Right.Equals(other.Right);
    public override int GetHashCode() => HashCode.Combine(Left, Right);
}