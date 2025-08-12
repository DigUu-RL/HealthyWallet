using System.Linq.Expressions;
using HealthyWallet.Infrastructure.Data.Entities;
using HealthyWallet.Infrastructure.Repository.DesignPattern.Specification;

namespace HealthyWallet.Infrastructure.Repository.Interfaces;

public interface IReadOnlyRepository<TEntity> where TEntity : EntityBase
{
    void With(params Expression<Func<TEntity, object>>[] expressions);
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity?> GetByReferenceIdAsync(Guid referenceId);
    Task<TEntity?> GetByAsync(Specification<TEntity> specification);
    IQueryable<TEntity> Query(Specification<TEntity>? specification = null, bool tracking = false);
}