using System.Linq.Expressions;
using HealthyWallet.Infrastructure.Data.Contexts;
using HealthyWallet.Infrastructure.Data.Entities;
using HealthyWallet.Infrastructure.Repository.DesignPattern.Specification;
using HealthyWallet.Infrastructure.Repository.DesignPattern.Specification.Abstractions;
using HealthyWallet.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthyWallet.Infrastructure.Repository.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase
{
    private readonly DbSet<TEntity> _set;
    private IQueryable<TEntity> _query;

    public BaseRepository(HealthyWalletContext context)
    {
        DbSet<TEntity> set = context.Set<TEntity>();

        _set = set;
        _query = set.AsQueryable();
    }

    public void With(params Expression<Func<TEntity, object>>[] expressions)
    {
        foreach (Expression<Func<TEntity, object>> expression in expressions)
        {
            _query = _query.Include(expression);
        }
    }

    public async Task<TEntity?> GetByIdAsync(int id) => await _query.SingleOrDefaultAsync(entity => entity.Id == id);
    public async Task<TEntity?> GetByReferenceIdAsync(Guid referenceId) => await _query.SingleOrDefaultAsync(entity => entity.ReferenceId == referenceId);
    public async Task<TEntity?> GetByAsync(Specification<TEntity> specification) => await  _query.SingleOrDefaultAsync(specification);
    public async Task CreateAsync(params TEntity[] entities) => await _set.AddRangeAsync(entities);
    public async Task CreateAsync(IEnumerable<TEntity> entities) => await _set.AddRangeAsync(entities);

    public async Task UpdateAsync(params TEntity[] entities)
    {
        foreach (TEntity entity in entities) entity.UpdatedAt = DateTime.UtcNow;
        await _set.AddRangeAsync(entities);
    }

    public async Task UpdateAsync(IEnumerable<TEntity> entities)
    {
        TEntity[] array = entities as TEntity[] ?? entities.ToArray();
        await UpdateAsync(array);
    }

    public Task DeleteAsync(params TEntity[] entities)
    {
        _set.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(IEnumerable<TEntity> entities)
    {
        _set.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public IQueryable<TEntity> Query(Specification<TEntity>? specification = null, bool tracking = false)
    {
        specification ??= new TrueSpecification<TEntity>();
        IQueryable<TEntity> query = tracking ? _query : _query.AsNoTracking();
        
        return query.Where(specification);
    }
}