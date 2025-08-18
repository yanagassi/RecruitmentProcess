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
        public async Task RegisterAsync_WithValidData_ShouldReturnSuccessResult()
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

            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _authService.RegisterAsync(registerDto);

            Assert.True(result.Success);
            Assert.Equal("User registered successfully", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ShouldReturnFailureResult()
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

            var result = await _authService.RegisterAsync(registerDto);

            Assert.False(result.Success);
            Assert.Equal("User with this email already exists", result.Message);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccessWithToken()
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

            _tokenServiceMock.Setup(x => x.GenerateToken(user))
                .Returns("test-jwt-token");

            var result = await _authService.LoginAsync(loginDto);

            Assert.True(result.Success);
            Assert.Equal("test-jwt-token", result.Token);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ShouldReturnFailureResult()
        {
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync((ApplicationUser)null);

            var result = await _authService.LoginAsync(loginDto);

            Assert.False(result.Success);
            Assert.Equal("Invalid email or password", result.Message);
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentUser_ShouldReturnFailureResult()
        {
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "Test123!"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync((ApplicationUser)null);

            var result = await _authService.LoginAsync(loginDto);

            Assert.False(result.Success);
            Assert.Equal("Invalid email or password", result.Message);
        }

        [Fact]
        public async Task GetCurrentUserAsync_WithValidUserId_ShouldReturnUser()
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

            var result = await _authService.GetCurrentUserAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task GetCurrentUserAsync_WithInvalidUserId_ShouldReturnNull()
        {
            var userId = "invalid-user-id";

            _userManagerMock.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync((ApplicationUser)null);

            var result = await _authService.GetCurrentUserAsync(userId);

            Assert.Null(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}