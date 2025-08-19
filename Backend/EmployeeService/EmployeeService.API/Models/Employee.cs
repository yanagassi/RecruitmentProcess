using System.ComponentModel.DataAnnotations;

namespace EmployeeService.API.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

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
        public int Age { get; set; }

        [Required]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;

        [StringLength(200)]
        public string Department { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; }

        public int? ManagerId { get; set; }
        public Employee? Manager { get; set; }

        public PermissionLevel PermissionLevel { get; set; } = PermissionLevel.Employee;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
        public ICollection<EmployeePhone> Phones { get; set; } = new List<EmployeePhone>();
    }
}