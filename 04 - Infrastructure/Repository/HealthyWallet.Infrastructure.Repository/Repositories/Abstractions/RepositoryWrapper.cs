using HealthyWallet.Infrastructure.Data.Entities.Identity;
using HealthyWallet.Infrastructure.Repository.Interfaces;
using HealthyWallet.Infrastructure.Repository.Interfaces.Abstractions;

namespace HealthyWallet.Infrastructure.Repository.Repositories.Abstractions;

public sealed class RepositoryWrapper(IBaseRepository<User> userRepository) : IRepositoryWrapper
{
    public IReadOnlyRepository<User> User => userRepository;
}