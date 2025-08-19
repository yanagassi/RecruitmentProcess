using EmployeeService.API.Models.DTOs;
using EmployeeService.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace EmployeeService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(EmployeeListResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<EmployeeListResponseDto>> GetAllEmployees()
        {
            var response = await _employeeService.GetAllEmployeesAsync();
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmployeeResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
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
        [ProducesResponseType(typeof(EmployeeResponseDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<EmployeeResponseDto>> CreateEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (createEmployeeDto.HireDate > DateTime.UtcNow)
                return BadRequest(new { message = "Hire date cannot be in the future" });

            if (createEmployeeDto.Age < 16)
                return BadRequest(new { message = "Employee must be at least 16 years old" });

            var currentUserEmail = User.Identity?.Name ?? 
                                 User.FindFirst(ClaimTypes.Email)?.Value ?? 
                                 User.FindFirst("email")?.Value ??
                                 User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
            
            if (string.IsNullOrEmpty(currentUserEmail))
                return BadRequest(new { message = "Unable to identify current user" });

            var response = await _employeeService.CreateEmployeeAsync(createEmployeeDto, currentUserEmail);

            if (!response.Success)
                return BadRequest(response);

            return CreatedAtAction(nameof(GetEmployeeById), new { id = response.Employee?.Id }, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmployeeResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
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
        [ProducesResponseType(typeof(EmployeeResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
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