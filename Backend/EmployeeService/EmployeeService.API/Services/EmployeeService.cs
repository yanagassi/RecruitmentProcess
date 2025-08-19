using EmployeeService.API.Data;
using EmployeeService.API.Models;
using EmployeeService.API.Models.DTOs;
using EmployeeService.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.API.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeListResponseDto> GetAllEmployeesAsync();
        Task<EmployeeResponseDto> GetEmployeeByIdAsync(int id);
        Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto, string currentUserEmail);
        Task<EmployeeResponseDto> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateEmployeeDto);
        Task<EmployeeResponseDto> DeleteEmployeeAsync(int id);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeListResponseDto> GetAllEmployeesAsync()
        {
            try
            {
                var employees = await _context.Employees.ToListAsync();
                var employeeDtos = employees.Select(e => MapToEmployeeDto(e)).ToList();

                return new EmployeeListResponseDto
                {
                    Success = true,
                    Message = "Employees retrieved successfully",
                    Employees = employeeDtos
                };
            }
            catch (Exception ex)
            {
                return new EmployeeListResponseDto
                {
                    Success = false,
                    Message = $"Error retrieving employees: {ex.Message}"
                };
            }
        }

        public async Task<EmployeeResponseDto> GetEmployeeByIdAsync(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);

                if (employee == null)
                {
                    return new EmployeeResponseDto
                    {
                        Success = false,
                        Message = "Employee not found"
                    };
                }

                return new EmployeeResponseDto
                {
                    Success = true,
                    Message = "Employee retrieved successfully",
                    Employee = MapToEmployeeDto(employee)
                };
            }
            catch (Exception ex)
            {
                return new EmployeeResponseDto
                {
                    Success = false,
                    Message = $"Error retrieving employee: {ex.Message}"
                };
            }
        }

        public async Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto, string currentUserEmail)
        {
            try
            {
                var existingEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.DocNumber == createEmployeeDto.DocNumber);

                if (existingEmployee != null)
                {
                    return new EmployeeResponseDto
                    {
                        Success = false,
                        Message = "An employee with this document number already exists"
                    };
                }

                var currentUser = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Email == currentUserEmail);

                if (currentUser != null)
                {
                    if ((currentUser.PermissionLevel == PermissionLevel.Employee && createEmployeeDto.PermissionLevel != PermissionLevel.Employee) ||
                        (currentUser.PermissionLevel == PermissionLevel.Leader && createEmployeeDto.PermissionLevel == PermissionLevel.Director))
                    {
                        return new EmployeeResponseDto
                        {
                            Success = false,
                            Message = "You don't have permission to create an employee with this permission level"
                        };
                    }
                }

                var employee = new Employee
                {
                    FirstName = createEmployeeDto.FirstName,
                    LastName = createEmployeeDto.LastName,
                    Email = createEmployeeDto.Email,
                    DocNumber = createEmployeeDto.DocNumber,
                    Age = createEmployeeDto.Age,
                    Position = createEmployeeDto.Position,
                    Department = createEmployeeDto.Department,
                    Salary = createEmployeeDto.Salary,
                    HireDate = DateTime.SpecifyKind(createEmployeeDto.HireDate, DateTimeKind.Utc),
                    ManagerId = createEmployeeDto.ManagerId,
                    PermissionLevel = createEmployeeDto.PermissionLevel
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                if (createEmployeeDto.Phones != null && createEmployeeDto.Phones.Any())
                {
                    foreach (var phoneDto in createEmployeeDto.Phones)
                    {
                        if (!string.IsNullOrEmpty(phoneDto.PhoneNumber))
                        {
                            var phone = new EmployeePhone
                            {
                                EmployeeId = employee.Id,
                                PhoneNumber = phoneDto.PhoneNumber,
                                PhoneType = phoneDto.PhoneType ?? "Mobile",
                                IsPrimary = phoneDto.IsPrimary,
                                CreatedAt = DateTime.UtcNow
                            };
                            _context.EmployeePhones.Add(phone);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return new EmployeeResponseDto
                {
                    Success = true,
                    Message = "Employee created successfully",
                    Employee = MapToEmployeeDto(employee)
                };
            }
            catch (Exception ex)
            {
                return new EmployeeResponseDto
                {
                    Success = false,
                    Message = $"Error creating employee: {ex.Message}"
                };
            }
        }

        public async Task<EmployeeResponseDto> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.Manager)
                    .Include(e => e.Phones)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    return new EmployeeResponseDto
                    {
                        Success = false,
                        Message = "Employee not found"
                    };
                }

                if (updateEmployeeDto.FirstName != null)
                    employee.FirstName = updateEmployeeDto.FirstName;

                if (updateEmployeeDto.LastName != null)
                    employee.LastName = updateEmployeeDto.LastName;

                if (updateEmployeeDto.Email != null)
                    employee.Email = updateEmployeeDto.Email;

                if (updateEmployeeDto.DocNumber != null)
                    employee.DocNumber = updateEmployeeDto.DocNumber;

                if (updateEmployeeDto.Age.HasValue)
                    employee.Age = updateEmployeeDto.Age.Value;

                if (updateEmployeeDto.Position != null)
                    employee.Position = updateEmployeeDto.Position;

                if (updateEmployeeDto.Department != null)
                    employee.Department = updateEmployeeDto.Department;

                if (updateEmployeeDto.Salary.HasValue)
                    employee.Salary = updateEmployeeDto.Salary.Value;

                if (updateEmployeeDto.HireDate.HasValue)
                    employee.HireDate = DateTime.SpecifyKind(updateEmployeeDto.HireDate.Value, DateTimeKind.Utc);

                if (updateEmployeeDto.ManagerId.HasValue)
                    employee.ManagerId = updateEmployeeDto.ManagerId.Value;

                if (updateEmployeeDto.PermissionLevel.HasValue)
                    employee.PermissionLevel = updateEmployeeDto.PermissionLevel.Value;

                if (updateEmployeeDto.Phones != null)
                {
                    _context.EmployeePhones.RemoveRange(employee.Phones);
                    
                    foreach (var phoneDto in updateEmployeeDto.Phones)
                    {
                        if (!string.IsNullOrEmpty(phoneDto.PhoneNumber))
                        {
                            var phone = new EmployeePhone
                            {
                                EmployeeId = employee.Id,
                                PhoneNumber = phoneDto.PhoneNumber,
                                PhoneType = phoneDto.PhoneType ?? "Mobile",
                                IsPrimary = phoneDto.IsPrimary ?? false,
                                CreatedAt = DateTime.UtcNow
                            };
                            _context.EmployeePhones.Add(phone);
                        }
                    }
                }

                employee.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                employee = await _context.Employees
                    .Include(e => e.Manager)
                    .Include(e => e.Phones)
                    .FirstOrDefaultAsync(e => e.Id == id);

                return new EmployeeResponseDto
                {
                    Success = true,
                    Message = "Employee updated successfully",
                    Employee = MapToEmployeeDto(employee!)
                };
            }
            catch (Exception ex)
            {
                return new EmployeeResponseDto
                {
                    Success = false,
                    Message = $"Error updating employee: {ex.Message}"
                };
            }
        }

        public async Task<EmployeeResponseDto> DeleteEmployeeAsync(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);

                if (employee == null)
                {
                    return new EmployeeResponseDto
                    {
                        Success = false,
                        Message = "Employee not found"
                    };
                }

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                return new EmployeeResponseDto
                {
                    Success = true,
                    Message = "Employee deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new EmployeeResponseDto
                {
                    Success = false,
                    Message = $"Error deleting employee: {ex.Message}"
                };
            }
        }

        private EmployeeDto MapToEmployeeDto(Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                DocNumber = employee.DocNumber,
                Age = employee.Age,
                Position = employee.Position,
                Department = employee.Department,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager != null ? $"{employee.Manager.FirstName} {employee.Manager.LastName}" : null,
                PermissionLevel = employee.PermissionLevel,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt,
                Phones = employee.Phones?.Select(p => new EmployeePhoneDto
                {
                    Id = p.Id,
                    PhoneNumber = p.PhoneNumber,
                    PhoneType = p.PhoneType,
                    IsPrimary = p.IsPrimary
                }).ToList() ?? new List<EmployeePhoneDto>()
            };
        }
    }
}