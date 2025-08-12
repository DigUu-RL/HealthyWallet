
using HealthyWallet.Infrastructure.Data.Contexts;
using HealthyWallet.Infrastructure.Data.Entities.Identity;
using HealthyWallet.Infrastructure.Repository.Interfaces.Identity;

namespace HealthyWallet.Infrastructure.Repository.Repositories.identity;

public class UserRepository(HealthyWalletContext context) : BaseRepository<User>(context), IUserRepository
{
    
}