using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.API.Controllers
{
    [ApiController]
    [Route("/")]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                message = "Recruitment Process API Gateway",
                status = "Running",
                timestamp = DateTime.UtcNow,
                services = new
                {
                    employeeService = "http://localhost:5022",
                    identityService = "http://localhost:5047"
                },
                endpoints = new
                {
                    employees = "/api/employees",
                    auth = "/api/auth"
                }
            });
        }
    }
}