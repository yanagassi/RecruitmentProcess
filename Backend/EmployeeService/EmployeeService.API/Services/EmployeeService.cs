using EmployeeService.API.Data;
using EmployeeService.API.Models;
using EmployeeService.API.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.API.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeListResponseDto> GetAllEmployeesAsync();
        Task<EmployeeResponseDto> GetEmployeeByIdAsync(int id);
        Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto);
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
                    // Employees já tem um valor padrão (new List<EmployeeDto>())
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
                        Message = $"Employee with ID {id} not found"
                        // Employee já é nullable
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
                    // Employee já é nullable
                };
            }
        }

        public async Task<EmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto)
        {
            try
            {
                if (await _context.Employees.AnyAsync(e => e.Email == createEmployeeDto.Email))
                {
                    return new EmployeeResponseDto
                    {
                        Success = false,
                        Message = $"Employee with email {createEmployeeDto.Email} already exists"
                        // Employee já é nullable
                    };
                }

                var employee = new Employee
                {
                    FirstName = createEmployeeDto.FirstName,
                    LastName = createEmployeeDto.LastName,
                    Email = createEmployeeDto.Email,
                    Phone = createEmployeeDto.Phone,
                    Age = createEmployeeDto.Age,
                    Position = createEmployeeDto.Position,
                    Department = createEmployeeDto.Department,
                    Salary = createEmployeeDto.Salary,
                    HireDate = createEmployeeDto.HireDate,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

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
                    // Employee já é nullable
                };
            }
        }

        public async Task<EmployeeResponseDto> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);

                if (employee == null)
                {
                    return new EmployeeResponseDto
                    {
                        Success = false,
                        Message = $"Employee with ID {id} not found"
                        // Employee já é nullable
                    };
                }

                if (!string.IsNullOrEmpty(updateEmployeeDto.Email) && 
                    updateEmployeeDto.Email != employee.Email && 
                    await _context.Employees.AnyAsync(e => e.Email == updateEmployeeDto.Email))
                {
                    return new EmployeeResponseDto
                    {
                        Success = false,
                        Message = $"Employee with email {updateEmployeeDto.Email} already exists"
                        // Employee já é nullable
                    };
                }

                // Update only the properties that are provided
                if (!string.IsNullOrEmpty(updateEmployeeDto.FirstName))
                    employee.FirstName = updateEmployeeDto.FirstName;

                if (!string.IsNullOrEmpty(updateEmployeeDto.LastName))
                    employee.LastName = updateEmployeeDto.LastName;

                if (!string.IsNullOrEmpty(updateEmployeeDto.Email))
                    employee.Email = updateEmployeeDto.Email;

                if (!string.IsNullOrEmpty(updateEmployeeDto.Phone))
                    employee.Phone = updateEmployeeDto.Phone;

                if (updateEmployeeDto.Age.HasValue)
                    employee.Age = updateEmployeeDto.Age.Value;

                if (!string.IsNullOrEmpty(updateEmployeeDto.Position))
                    employee.Position = updateEmployeeDto.Position;

                if (updateEmployeeDto.Department != null)
                    employee.Department = updateEmployeeDto.Department;

                if (updateEmployeeDto.Salary.HasValue)
                    employee.Salary = updateEmployeeDto.Salary.Value;

                if (updateEmployeeDto.HireDate.HasValue)
                    employee.HireDate = updateEmployeeDto.HireDate.Value;

                employee.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new EmployeeResponseDto
                {
                    Success = true,
                    Message = "Employee updated successfully",
                    Employee = MapToEmployeeDto(employee)
                };
            }
            catch (Exception ex)
            {
                return new EmployeeResponseDto
                {
                    Success = false,
                    Message = $"Error updating employee: {ex.Message}"
                    // Employee já é nullable
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
                        Message = $"Employee with ID {id} not found"
                        // Employee já é nullable
                    };
                }

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                return new EmployeeResponseDto
                {
                    Success = true,
                    Message = "Employee deleted successfully"
                    // Employee já é nullable e deve permanecer null após exclusão
                };
            }
            catch (Exception ex)
            {
                return new EmployeeResponseDto
                {
                    Success = false,
                    Message = $"Error deleting employee: {ex.Message}"
                    // Employee já é nullable
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
                Phone = employee.Phone,
                Age = employee.Age,
                Position = employee.Position,
                Department = employee.Department,
                Salary = employee.Salary,
                HireDate = employee.HireDate,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt
            };
        }
    }
}