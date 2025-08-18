using IdentityService.API.Models;
using IdentityService.API.Services;
using IdentityService.API.Settings;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace IdentityService.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IOptions<JwtSettings>> _jwtOptionsMock;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            var jwtSettings = new JwtSettings
            {
                Secret = "SuperSecretKeyThatIsAtLeast32CharactersLong123456789",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpiryMinutes = 60
            };

            _jwtOptionsMock = new Mock<IOptions<JwtSettings>>();
            _jwtOptionsMock.Setup(x => x.Value).Returns(jwtSettings);

            _tokenService = new TokenService(_jwtOptionsMock.Object);
        }

        [Fact]
        public void GenerateToken_WithValidUser_ShouldReturnValidJwtToken()
        {
            var user = new ApplicationUser
            {
                Id = "test-id",
                Email = "test@example.com",
                UserName = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            var token = _tokenService.GenerateJwtToken(user);

            Assert.NotNull(token);
            Assert.NotEmpty(token);

            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(token));

            var jsonToken = handler.ReadJwtToken(token);
            
            Assert.Equal("test-id", jsonToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value);
            Assert.Equal("test@example.com", jsonToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value);
            Assert.Equal("Test", jsonToken.Claims.FirstOrDefault(x => x.Type == "firstName")?.Value);
            Assert.Equal("User", jsonToken.Claims.FirstOrDefault(x => x.Type == "lastName")?.Value);

            Assert.Equal("TestIssuer", jsonToken.Issuer);
            Assert.Contains("TestAudience", jsonToken.Audiences);

            Assert.True(jsonToken.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void GenerateToken_WithNullUser_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _tokenService.GenerateJwtToken(null));
        }

        [Fact]
        public void GenerateToken_WithUserWithoutId_ShouldThrowArgumentException()
        {
            var user = new ApplicationUser
            {
                Id = null,
                Email = "test@example.com",
                UserName = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            Assert.Throws<ArgumentException>(() => _tokenService.GenerateJwtToken(user));
        }

        [Fact]
        public void GenerateToken_WithUserWithoutEmail_ShouldThrowArgumentException()
        {
            var user = new ApplicationUser
            {
                Id = "test-id",
                Email = null,
                UserName = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            Assert.Throws<ArgumentException>(() => _tokenService.GenerateJwtToken(user));
        }

        [Fact]
        public void GenerateToken_TokenShouldExpireInOneHour()
        {
            var user = new ApplicationUser
            {
                Id = "test-id",
                Email = "test@example.com",
                UserName = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            var beforeGeneration = DateTime.UtcNow;

            var token = _tokenService.GenerateJwtToken(user);

            var afterGeneration = DateTime.UtcNow;

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);

            var expectedExpiry = beforeGeneration.AddHours(1);
            var actualExpiry = jsonToken.ValidTo;

            Assert.True(actualExpiry >= expectedExpiry.AddSeconds(-5));
            Assert.True(actualExpiry <= afterGeneration.AddHours(1).AddSeconds(5));
        }

        [Fact]
        public void GenerateToken_ShouldIncludeAllRequiredClaims()
        {
            var user = new ApplicationUser
            {
                Id = "test-id-123",
                Email = "john.doe@example.com",
                UserName = "john.doe@example.com",
                FirstName = "John",
                LastName = "Doe"
            };

            var token = _tokenService.GenerateJwtToken(user);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var claims = jsonToken.Claims.ToList();

            Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "test-id-123");
            Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "john.doe@example.com");
            Assert.Contains(claims, c => c.Type == "firstName" && c.Value == "John");
            Assert.Contains(claims, c => c.Type == "lastName" && c.Value == "Doe");
            Assert.Contains(claims, c => c.Type == JwtRegisteredClaimNames.Jti);
        }
    }
}