using IdentityService.API.Data;
using IdentityService.API.Models;
using IdentityService.API.Models.DTOs;
using IdentityService.API.Services;
using IdentityService.Tests.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace IdentityService.Tests.Services
{
    public class AuthServiceTests : IDisposable
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly AuthService _authService;
        private readonly ApplicationDbContext _context;
        private readonly Mock<ITokenService> _tokenServiceMock;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            _userManagerMock = CreateMockUserManager();
            _tokenServiceMock = CreateMockTokenService();

            _authService = new AuthService(_userManagerMock.Object, _tokenServiceMock.Object);
        }

        private Mock<UserManager<ApplicationUser>> CreateMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<ITokenService> CreateMockTokenService()
        {
            return new Mock<ITokenService>();
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ShouldReturnAuthResponse()
        {
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "Test123!",
                FirstName = "Test",
                LastName = "User"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(registerDto.Email))
                .ReturnsAsync((ApplicationUser)null);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), registerDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _tokenServiceMock.Setup(x => x.GenerateJwtToken(It.IsAny<ApplicationUser>()))
                .Returns("test-jwt-token");

            var result = await _authService.RegisterAsync(registerDto);

            Assert.NotNull(result);
            Assert.Equal("test-jwt-token", result.Token);
            Assert.NotNull(result.User);
            Assert.Equal("test@example.com", result.User.Email);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ShouldThrowException()
        {
            var registerDto = new RegisterDto
            {
                Email = "existing@example.com",
                Password = "Test123!",
                FirstName = "Test",
                LastName = "User"
            };

            var existingUser = new ApplicationUser { Email = registerDto.Email };
            _userManagerMock.Setup(x => x.FindByEmailAsync(registerDto.Email))
                .ReturnsAsync(existingUser);

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _authService.RegisterAsync(registerDto));
            Assert.Equal("User with this email already exists", exception.Message);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnAuthResponse()
        {
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "Test123!"
            };

            var user = new ApplicationUser
            {
                Id = "test-id",
                Email = loginDto.Email,
                FirstName = "Test",
                LastName = "User"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
                .ReturnsAsync(true);

            _tokenServiceMock.Setup(x => x.GenerateJwtToken(user))
                .Returns("test-jwt-token");

            var result = await _authService.LoginAsync(loginDto);

            Assert.NotNull(result);
            Assert.Equal("test-jwt-token", result.Token);
            Assert.NotNull(result.User);
            Assert.Equal("test@example.com", result.User.Email);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ShouldThrowException()
        {
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync((ApplicationUser)null);

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _authService.LoginAsync(loginDto));
            Assert.Equal("Invalid email or password", exception.Message);
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentUser_ShouldThrowException()
        {
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "Test123!"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync((ApplicationUser)null);

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _authService.LoginAsync(loginDto));
            Assert.Equal("Invalid email or password", exception.Message);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidUserId_ShouldReturnUser()
        {
            var userId = "test-user-id";
            var user = new ApplicationUser
            {
                Id = userId,
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var result = await _authService.GetUserByIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithInvalidUserId_ShouldThrowException()
        {
            var userId = "invalid-user-id";

            _userManagerMock.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync((ApplicationUser)null);

            await Assert.ThrowsAsync<ApplicationException>(() => _authService.GetUserByIdAsync(userId));
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}