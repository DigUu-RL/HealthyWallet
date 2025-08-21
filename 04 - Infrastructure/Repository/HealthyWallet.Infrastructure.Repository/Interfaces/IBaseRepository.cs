using HealthyWallet.Infrastructure.Data.Entities;

namespace HealthyWallet.Infrastructure.Repository.Interfaces;

public interface IBaseRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : EntityBase
{
    Task CreateAsync(params TEntity[] entities);
    Task CreateAsync(IEnumerable<TEntity> entities);
    Task UpdateAsync(params TEntity[] entities);
    Task UpdateAsync(IEnumerable<TEntity> entities);
    Task CreateOrUpdateAsync(params TEntity[] entities);
    Task CreateOrUpdateAsync(IEnumerable<TEntity> entities);
    Task DeleteAsync(params TEntity[] entities);
    Task DeleteAsync(IEnumerable<TEntity> entities);
}
