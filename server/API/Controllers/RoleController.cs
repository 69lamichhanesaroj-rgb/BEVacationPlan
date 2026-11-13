using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // any valid token. No role is specified, so both employees and accountants can call it.
public class RoleController(RoleService roleService) : ControllerBase
{
    [HttpGet]
    public List<RoleDto> GetRoles()
    {
        return roleService.GetRoles();
    }
}