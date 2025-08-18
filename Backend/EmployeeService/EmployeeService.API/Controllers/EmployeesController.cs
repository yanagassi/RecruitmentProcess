using EmployeeService.API.Models.DTOs;
using EmployeeService.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<EmployeeListResponseDto>> GetAllEmployees()
        {
            var response = await _employeeService.GetAllEmployeesAsync();
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid employee ID" });

            var response = await _employeeService.GetEmployeeByIdAsync(id);

            if (!response.Success || response.Employee == null)
                return NotFound(response);

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeResponseDto>> CreateEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (createEmployeeDto.HireDate > DateTime.UtcNow)
                return BadRequest(new { message = "Hire date cannot be in the future" });

            if (createEmployeeDto.Age < 16)
                return BadRequest(new { message = "Employee must be at least 16 years old" });

            var response = await _employeeService.CreateEmployeeAsync(createEmployeeDto);

            if (!response.Success)
                return BadRequest(response);

            return CreatedAtAction(nameof(GetEmployeeById), new { id = response.Employee?.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeResponseDto>> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid employee ID" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (updateEmployeeDto.HireDate.HasValue && updateEmployeeDto.HireDate > DateTime.UtcNow)
                return BadRequest(new { message = "Hire date cannot be in the future" });

            if (updateEmployeeDto.Age.HasValue && updateEmployeeDto.Age < 16)
                return BadRequest(new { message = "Employee must be at least 16 years old" });

            var response = await _employeeService.UpdateEmployeeAsync(id, updateEmployeeDto);

            if (!response.Success)
            {
                if (response.Message.Contains("not found"))
                    return NotFound(response);

                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<EmployeeResponseDto>> DeleteEmployee(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid employee ID" });

            var response = await _employeeService.DeleteEmployeeAsync(id);

            if (!response.Success)
            {
                if (response.Message.Contains("not found"))
                    return NotFound(response);

                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}