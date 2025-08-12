using System.Linq.Expressions;

namespace HealthyWallet.Infrastructure.Repository.DesignPattern.Specification.Abstractions;

public sealed class TrueSpecification<T> : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression() => _ => true;
    
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return GetType() == obj.GetType();
    }

    public override int GetHashCode() => typeof(TrueSpecification<T>).GetHashCode();
}