using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Security;
using Infra.Entities;
using Service.Security;


namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeeController(EmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = $"{RoleNames.SystemOwner},{RoleNames.Accountant}")]
    public List<EmployeeDto> GetEmployees()
    {
        return employeeService.GetEmployees();
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.SystemOwner)]
    public IActionResult CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        try
        {
            var id = employeeService.CreateEmployee(request);
            return Ok(id);
        }
        catch (ArgumentException ex)
        {
            return BadRequest( new {message = ex.Message});
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPost("{id:int}/deactivate")]
    [Authorize(Roles = RoleNames.SystemOwner)]
    public IActionResult Deactivate(int id, [FromBody] DeactivateEmployeeRequest request)
    {
        try
        {
            employeeService.DeactivateEmployee(id, request.EndDate);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}