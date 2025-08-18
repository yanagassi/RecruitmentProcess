using System.ComponentModel.DataAnnotations;

namespace EmployeeService.API.DTOs
{
    public class EmployeePhoneDto
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string PhoneType { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }

    public class CreateEmployeePhoneDto
    {
        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(50)]
        public string PhoneType { get; set; } = "Mobile";

        public bool IsPrimary { get; set; } = false;
    }

    public class UpdateEmployeePhoneDto
    {
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(50)]
        public string? PhoneType { get; set; }

        public bool? IsPrimary { get; set; }
    }
}