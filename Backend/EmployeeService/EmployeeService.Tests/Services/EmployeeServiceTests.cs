using EmployeeService.API.Data;
using EmployeeService.API.Models;
using EmployeeService.API.Models.DTOs;
using EmployeeService.API.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeeService.Tests.Services
{
    public class EmployeeServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly API.Services.EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _employeeService = new API.Services.EmployeeService(_context);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var adminEmployee = new Employee
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@test.com",
                DocNumber = "12345678901",
                Age = 30,
                Position = "Administrator",
                Department = "IT",
                Salary = 5000,
                HireDate = DateTime.UtcNow.AddYears(-2),
                PermissionLevel = PermissionLevel.Director,
                CreatedAt = DateTime.UtcNow
            };

            var managerEmployee = new Employee
            {
                Id = 2,
                FirstName = "Manager",
                LastName = "User",
                Email = "manager@test.com",
                DocNumber = "12345678902",
                Age = 35,
                Position = "Manager",
                Department = "HR",
                Salary = 4000,
                HireDate = DateTime.UtcNow.AddYears(-1),
                PermissionLevel = PermissionLevel.Leader,
                CreatedAt = DateTime.UtcNow
            };

            var regularEmployee = new Employee
            {
                Id = 3,
                FirstName = "Regular",
                LastName = "User",
                Email = "regular@test.com",
                DocNumber = "12345678903",
                Age = 25,
                Position = "Developer",
                Department = "IT",
                Salary = 3000,
                HireDate = DateTime.UtcNow.AddMonths(-6),
                PermissionLevel = PermissionLevel.Employee,
                CreatedAt = DateTime.UtcNow
            };

            _context.Employees.AddRange(adminEmployee, managerEmployee, regularEmployee);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllEmployeesAsync_ShouldReturnAllEmployees()
        {
            var result = await _employeeService.GetAllEmployeesAsync();

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(3, result.Employees.Count);
            Assert.Contains(result.Employees, e => e.FirstName == "Admin" && e.LastName == "User");
            Assert.Contains(result.Employees, e => e.FirstName == "Manager" && e.LastName == "User");
            Assert.Contains(result.Employees, e => e.FirstName == "Regular" && e.LastName == "User");
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_WithValidId_ShouldReturnEmployee()
        {
            var result = await _employeeService.GetEmployeeByIdAsync(1);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Employee);
            Assert.Equal("Admin", result.Employee.FirstName);
            Assert.Equal("admin@test.com", result.Employee.Email);
            Assert.Equal(PermissionLevel.Director, result.Employee.PermissionLevel);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            var result = await _employeeService.GetEmployeeByIdAsync(999);

            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Null(result.Employee);
            Assert.Equal("Employee not found", result.Message);
        }

        [Fact]
        public async Task CreateEmployeeAsync_WithValidData_ShouldCreateEmployee()
        {
            var createDto = new CreateEmployeeDto
            {
                FirstName = "New",
                LastName = "Employee",
                Email = "new@test.com",
                DocNumber = "12345678904",
                Age = 28,
                Position = "Developer",
                Department = "IT",
                Salary = 3500,
                HireDate = DateTime.UtcNow,
                PermissionLevel = PermissionLevel.Employee
            };

            var result = await _employeeService.CreateEmployeeAsync(createDto, "admin@test.com");

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Employee);
            Assert.Equal("New", result.Employee.FirstName);
            Assert.Equal("new@test.com", result.Employee.Email);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_WithValidData_ShouldUpdateEmployee()
        {
            var updateDto = new UpdateEmployeeDto
            {
                FirstName = "John Updated",
                LastName = "Doe Updated",
                Email = "john.updated@test.com",
                Position = "Senior Developer"
            };

            var result = await _employeeService.UpdateEmployeeAsync(1, updateDto);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Employee);
            Assert.Equal("John Updated", result.Employee.FirstName);
            Assert.Equal("john.updated@test.com", result.Employee.Email);
        }

        [Fact]
        public async Task DeleteEmployeeAsync_WithValidId_ShouldDeleteEmployee()
        {
            var result = await _employeeService.DeleteEmployeeAsync(3);

            Assert.NotNull(result);
            Assert.True(result.Success);

            var deletedEmployee = await _employeeService.GetEmployeeByIdAsync(3);
            Assert.NotNull(deletedEmployee);
            Assert.False(deletedEmployee.Success);
            Assert.Null(deletedEmployee.Employee);
        }

        [Fact]
        public async Task DeleteEmployeeAsync_WithInvalidId_ShouldReturnFalse()
        {
            var result = await _employeeService.DeleteEmployeeAsync(999);

            Assert.NotNull(result);
            Assert.False(result.Success);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}