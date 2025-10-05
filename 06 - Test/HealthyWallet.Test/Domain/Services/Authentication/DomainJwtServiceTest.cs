using System.Text;
using HealthyWallet.Domain.Interfaces.Authentication;
using HealthyWallet.Domain.Models.Authentication;
using HealthyWallet.Domain.Services.Authentication;
using HealthyWallet.Infrastructure.Data.Entities.Identity;
using HealthyWallet.Infrastructure.Data.Enums.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Moq;
using StackExchange.Redis;

namespace HealthyWallet.Test.Domain.Services.Authentication;

public class DomainJwtServiceTest
{
    private readonly Mock<IConfiguration> _configurationMock = new();
    private readonly Mock<IConfigurationSection> _jwtSectionMock = new();
    private readonly Mock<IDatabase> _redisMock = new();
    private readonly Mock<IHostEnvironment> _envMock = new();
    
    private readonly IDomainJwtService _jwtService;

    public DomainJwtServiceTest()
    {
        IConfigurationSection audience = Mock.Of<IConfigurationSection>(section => section.Value == "7F6F2530-B862-446E-B051-10A4FBA04C72");
        IConfigurationSection issuer = Mock.Of<IConfigurationSection>(section => section.Value == "60A374BB-A6F4-4485-AB20-BF6DFA530DC7");
        IConfigurationSection secretKey = Mock.Of<IConfigurationSection>(section => section.Value == "6zH9pYvQpXJrMfT3nB2qWd8Ku4sYtV5Z");

        _jwtSectionMock.Setup(section => section.GetSection("Audience")).Returns(audience);
        _jwtSectionMock.Setup(section => section.GetSection("Issuer")).Returns(issuer);
        _jwtSectionMock.Setup(section => section.GetSection("SecretKey")).Returns(secretKey);

        _configurationMock.Setup(configuration => configuration.GetSection("Jwt")).Returns(_jwtSectionMock.Object);

        var parameters = new TokenValidationParameters
        {
            ValidAudience = _jwtSectionMock.Object.GetSection("Audience").Value,
            ValidIssuer = _jwtSectionMock.Object.GetSection("Issuer").Value,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSectionMock.Object.GetSection("SecretKey").Value!)
            )
        };

        _jwtService = new DomainJwtService(_configurationMock.Object, parameters, _redisMock.Object, _envMock.Object);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidToken()
    {
        var user = new User
        {
            Id = 1,
            ReferenceId = Guid.NewGuid(),
            Name = "Test User",
            UserName = "test-user",
            Email = "test@email.com",
            Type = UserType.Administrator,
            Claims = new List<UserClaim>(),
            PasswordHash = string.Empty
        };

        AccessTokenModel accessToken = _jwtService.GenerateToken(user);

        Assert.NotNull(accessToken);
        Assert.False(string.IsNullOrEmpty(accessToken.Token));
        Assert.True(accessToken.ExpiresIn > DateTime.UtcNow);
    }
    
    [Fact]
    public async Task ValidateTokenAsync_ShouldReturnUser_WhenTokenIsValid()
    {
        var user = new User
        {
            Id = 1,
            ReferenceId = Guid.NewGuid(),
            Name = "Test User",
            UserName = "test-user",
            Email = "test@email.com",
            Type = UserType.Administrator,
            Claims = new List<UserClaim>(),
            PasswordHash = string.Empty
        };
        
        AccessTokenModel accessToken = _jwtService.GenerateToken(user);
        User validatedUser = await _jwtService.ValidateTokenAsync(accessToken.Token);
        
        Assert.NotNull(validatedUser);
        Assert.Equal(user.Id, validatedUser.Id);
        Assert.Equal(user.ReferenceId, validatedUser.ReferenceId);
        Assert.Equal(user.Name, validatedUser.Name);
        Assert.Equal(user.UserName, validatedUser.UserName);
        Assert.Equal(user.Email, validatedUser.Email);
        Assert.Equal(user.Type, validatedUser.Type);
    }
}