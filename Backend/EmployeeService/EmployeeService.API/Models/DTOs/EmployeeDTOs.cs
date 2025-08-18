using System.ComponentModel.DataAnnotations;
using EmployeeService.API.DTOs;

namespace EmployeeService.API.Models.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DocNumber { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Position { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public PermissionLevel PermissionLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<EmployeePhoneDto> Phones { get; set; } = new List<EmployeePhoneDto>();
    }

    public class CreateEmployeeDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string DocNumber { get; set; } = string.Empty;

        [Required]
        [Range(16, 100)]
        public int Age { get; set; }

        [Required]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;

        [StringLength(200)]
        public string Department { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        public int? ManagerId { get; set; }

        public PermissionLevel PermissionLevel { get; set; } = PermissionLevel.Employee;

        public List<CreateEmployeePhoneDto> Phones { get; set; } = new List<CreateEmployeePhoneDto>();
    }

    public class UpdateEmployeeDto
    {
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? DocNumber { get; set; }

        [Range(16, 100)]
        public int? Age { get; set; }

        [StringLength(100)]
        public string? Position { get; set; }

        [StringLength(200)]
        public string? Department { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Salary { get; set; }

        public DateTime? HireDate { get; set; }

        public int? ManagerId { get; set; }

        public PermissionLevel? PermissionLevel { get; set; }

        public List<UpdateEmployeePhoneDto>? Phones { get; set; }
    }

    public class EmployeeResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public EmployeeDto? Employee { get; set; }
    }

    public class EmployeeListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
    }
}