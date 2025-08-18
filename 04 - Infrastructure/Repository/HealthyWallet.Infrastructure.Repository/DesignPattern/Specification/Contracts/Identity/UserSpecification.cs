using HealthyWallet.Infrastructure.Data.Entities.Identity;
using HealthyWallet.Infrastructure.Repository.DesignPattern.Specification.Abstractions;

namespace HealthyWallet.Infrastructure.Repository.DesignPattern.Specification.Contracts.Identity;

public static class UserSpecification
{
    public static Specification<User> ById(long id) => new AdHocSpecification<User>(user => user.Id == id);
    public static Specification<User> ByReferenceId(Guid referenceId) => new AdHocSpecification<User>(user => user.ReferenceId == referenceId);
    public static Specification<User> ByName(string name) => new AdHocSpecification<User>(user => user.Name == name);
    public static Specification<User> ByUserName(string username) => new AdHocSpecification<User>(user => user.UserName == username);
    public static Specification<User> ByEmail(string email) => new AdHocSpecification<User>(user => user.Email == email);
}