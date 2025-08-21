using System.Linq.Expressions;
using HealthyWallet.Infrastructure.Data.Entities;
using HealthyWallet.Infrastructure.Repository.DesignPattern.Specification;

namespace HealthyWallet.Infrastructure.Repository.Interfaces;

/// <summary>
/// Provides read-only access to entities of type <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TEntity">The entity type, which must derive from <see cref="EntityBase"/>.</typeparam>
public interface IReadOnlyRepository<TEntity> where TEntity : EntityBase
{
    /// <summary>
    /// Configures the repository to include related entities when retrieving data.
    /// </summary>
    /// <param name="expressions">Expressions specifying the related entities to include.</param>
    void With(params Expression<Func<TEntity, object>>[] expressions);

    /// <summary>
    /// Retrieves an entity by its primary key identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The entity if found; otherwise, <c>null</c>.</returns>
    Task<TEntity?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves an entity by its reference identifier (GUID).
    /// </summary>
    /// <param name="referenceId">The unique reference identifier of the entity.</param>
    /// <returns>The entity if found; otherwise, <c>null</c>.</returns>
    Task<TEntity?> GetByReferenceIdAsync(Guid referenceId);

    /// <summary>
    /// Retrieves a single entity that matches the given specification.
    /// </summary>
    /// <param name="specification">The specification defining the filtering criteria.</param>
    /// <returns>The entity if found; otherwise, <c>null</c>.</returns>
    Task<TEntity?> GetByAsync(Specification<TEntity> specification);

    /// <summary>
    /// Returns a queryable collection of entities, optionally filtered by a specification.
    /// </summary>
    /// <param name="specification">The specification defining the filtering criteria. If <c>null</c>, no filtering is applied.</param>
    /// <param name="tracking">Indicates whether entity tracking should be enabled.</param>
    /// <returns>An <see cref="IQueryable{TEntity}"/> representing the query.</returns>
    IQueryable<TEntity> Query(Specification<TEntity>? specification = null, bool tracking = false);
}
