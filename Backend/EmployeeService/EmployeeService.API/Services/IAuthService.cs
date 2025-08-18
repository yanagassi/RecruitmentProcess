using EmployeeService.API.Models.DTOs;

namespace EmployeeService.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        string GenerateJwtToken(string email, string firstName, string lastName);
    }
}