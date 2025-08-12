using System.Linq.Expressions;
using HealthyWallet.Infrastructure.Repository.DesignPattern.Specification.Abstractions;

namespace HealthyWallet.Infrastructure.Repository.DesignPattern.Specification;

public abstract class Specification<T>
{
    public static implicit operator Expression<Func<T, bool>>(Specification<T> specification) => specification.ToExpression();
    public static implicit operator Func<T, bool>(Specification<T> specification) => specification.ToExpression().Compile();
    
    public abstract Expression<Func<T, bool>> ToExpression();
    
    public static Specification<T> operator &(Specification<T> left, Specification<T> right) => new AndSpecification<T>(left, right);
    public static Specification<T> operator |(Specification<T> left, Specification<T> right) => new OrSpecification<T>(left, right);
    public static Specification<T> operator !(Specification<T> specification) => new NotSpecification<T>(specification);

    public static bool operator false(Specification<T> _) => false;
    public static bool operator true(Specification<T> _) => true;
}