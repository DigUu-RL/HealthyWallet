using HealthyWallet.Domain.Exceptions.Abstractions.Authentication;
using HealthyWallet.Domain.Helpers;
using HealthyWallet.Domain.Interfaces.Authentication;
using HealthyWallet.Domain.Models.Authentication;
using HealthyWallet.Domain.Requests.Authentication;
using HealthyWallet.Domain.Services.Authentication;
using HealthyWallet.Infrastructure.Data.Entities.Identity;
using HealthyWallet.Infrastructure.Data.Enums.Identity;
using HealthyWallet.Infrastructure.Repository.DesignPattern.Specification;
using HealthyWallet.Infrastructure.Repository.Interfaces;
using Moq;

namespace HealthyWallet.Test.Domain.Services.Authentication;

public class DomainAuthenticationServiceTest
{
    private readonly Mock<IDomainJwtService> _jwtServiceMock = new();
    private readonly Mock<IReadOnlyRepository<User>> _userRepositoryMock = new();

    private readonly IDomainAuthenticationService _authenticationService;

    private const string Username = "tester-nickname";

    public DomainAuthenticationServiceTest()
    {
        _authenticationService = new DomainAuthenticationService(
            _jwtServiceMock.Object,
            _userRepositoryMock.Object
        );
    }
    
    [Fact]
    public async Task SignInAsync_ShouldReturnAccessToken()
    {
        const string password = "top-secret-password";
        
        var request = new SignInRequest(Username, password);
        
        User user = CreateTestUser(password);
        AccessTokenModel expectedToken = CreateTestToken();

        _userRepositoryMock
            .Setup(repository => repository.GetByAsync(It.IsAny<Specification<User>>()))
            .ReturnsAsync(user);

        _jwtServiceMock
            .Setup(s => s.GenerateToken(It.IsAny<User>()))
            .Returns(expectedToken);

        AccessTokenModel accessToken = await _authenticationService.SignInAsync(request);

        Assert.NotNull(accessToken);
        Assert.Equal(expectedToken.Token, accessToken.Token);
        Assert.True(accessToken.ExpiresIn > DateTime.UtcNow);
    }
    
    [Fact]
    public async Task ValidateTokenAsync_ShouldReturnValidUser()
    {
        const string token = "valid-token";
        
        User user = CreateTestUser();

        _jwtServiceMock
            .Setup(service => service.ValidateTokenAsync(token))
            .ReturnsAsync(user);

        User result = await _authenticationService.ValidateTokenAsync("Bearer", token);

        Assert.NotNull(result);
        Assert.Equal(user.UserName, result.UserName);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task ValidateTokenAsync_InvalidType_ShouldThrowException()
    {
        await Assert.ThrowsAsync<InvalidTokenException>(async () =>
            await _authenticationService.ValidateTokenAsync("InvalidType", "any-token")
        );
    }

    [Fact]
    public async Task ValidateTokenAsync_InvalidToken_ShouldThrowException()
    {
        const string token = "expired-token";

        _jwtServiceMock
            .Setup(service => service.ValidateTokenAsync(token))
            .ThrowsAsync(new InvalidTokenException("Token invalid"));

        await Assert.ThrowsAsync<InvalidTokenException>(async () =>
            await _authenticationService.ValidateTokenAsync("Bearer", token)
        );
    }
    
    private static User CreateTestUser(string? password = null) => new()
    {
        Name = "Tester",
        UserName = Username,
        Email = "tester@healthy.wallet.com",
        Claims = new List<UserClaim>(),
        Type = UserType.Physical,
        PasswordHash = password is not null ? PasswordHelper.Hash(password) : string.Empty
    };

    private static AccessTokenModel CreateTestToken() => new("fake-jwt-token", DateTime.UtcNow.AddMinutes(15));
}
