using System.ComponentModel.DataAnnotations;

namespace EmployeeService.API.Models
{
    public class EmployeePhone
    {
        public int Id { get; set; }
        
        [Required]
        public int EmployeeId { get; set; }
        
        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string PhoneType { get; set; } = "Mobile";
        
        public bool IsPrimary { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public Employee Employee { get; set; } = null!;
    }
}