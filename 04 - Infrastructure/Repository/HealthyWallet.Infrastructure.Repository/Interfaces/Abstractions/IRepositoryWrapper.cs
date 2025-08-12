using HealthyWallet.Infrastructure.Data.Entities.Identity;

namespace HealthyWallet.Infrastructure.Repository.Interfaces.Abstractions;

public interface IRepositoryWrapper
{
    public IReadOnlyRepository<User> User { get; }
}