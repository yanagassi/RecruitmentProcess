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

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = 1,
                    Name = "João Silva",
                    Email = "joao.silva@company.com",
                    Position = "CEO",
                    DocNumber = "123.456.789-00",
                    PermissionLevel = PermissionLevel.Admin,
                    ManagerId = null
                },
                new Employee
                {
                    Id = 2,
                    Name = "Maria Santos",
                    Email = "maria.santos@company.com",
                    Position = "CTO",
                    DocNumber = "987.654.321-00",
                    PermissionLevel = PermissionLevel.Manager,
                    ManagerId = 1
                },
                new Employee
                {
                    Id = 3,
                    Name = "Pedro Costa",
                    Email = "pedro.costa@company.com",
                    Position = "Developer",
                    DocNumber = "456.789.123-00",
                    PermissionLevel = PermissionLevel.Employee,
                    ManagerId = 2
                }
            };

            _context.Employees.AddRange(employees);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllEmployeesAsync_ShouldReturnAllEmployees()
        {
            var result = await _employeeService.GetAllEmployeesAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_WithValidId_ShouldReturnEmployee()
        {
            var result = await _employeeService.GetEmployeeByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            var result = await _employeeService.GetEmployeeByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateEmployeeAsync_WithValidData_ShouldCreateEmployee()
        {
            var createDto = new CreateEmployeeDto
            {
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice.johnson@test.com",
                DocNumber = "33333333333",
                Age = 25,
                Position = "Analyst",
                Department = "Finance",
                Salary = 55000,
                PermissionLevel = PermissionLevel.Employee
            };

            var result = await _employeeService.CreateEmployeeAsync(createDto);

            Assert.NotNull(result);
            Assert.Equal("Alice", result.FirstName);
            Assert.Equal("alice.johnson@test.com", result.Email);

            var employeeInDb = await _context.Employees.FindAsync(result.Id);
            Assert.NotNull(employeeInDb);
            Assert.Equal("Alice", employeeInDb.FirstName);
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
            Assert.Equal("John Updated", result.FirstName);
            Assert.Equal("john.updated@test.com", result.Email);
        }

        [Fact]
        public async Task DeleteEmployeeAsync_WithValidId_ShouldDeleteEmployee()
        {
            var result = await _employeeService.DeleteEmployeeAsync(1);

            Assert.True(result);

            var deletedEmployee = await _context.Employees.FindAsync(1);
            Assert.Null(deletedEmployee);
        }

        [Fact]
        public async Task DeleteEmployeeAsync_WithInvalidId_ShouldReturnFalse()
        {
            var result = await _employeeService.DeleteEmployeeAsync(999);

            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}