using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Security;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleNames.Accountant)]
public class UserController(UserService userService, RoleService roleService) : ControllerBase
{
    private int ActorId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) // This is the ID of the user making the request, extracted from the JWT token.
        ? id
        : throw new UnauthorizedAccessException("User ID not found in token.");

    [HttpGet]
    public List<UserDto> GetUsers()
    {
        return userService.GetUsers();
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        if (roleService.GetRole(request.RoleId) is null)
            return BadRequest(new { message = "Invalid role ID" });

        try
        {
            userService.CreateUser(request.Username, request.RoleId, request.Password);
            return NoContent(); // 204 No Content is a standard response for a successful POST that doesn’t return any data.
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message }); // 409 Conflict for a duplicate username.
        }
    }

    [HttpPut("{id:int}")] //makes the route accept only whole numbers.
    public IActionResult UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        if (userService.GetUser(id) is null)
            return NotFound(new { message = "User not found" });

        if (roleService.GetRole(request.RoleId) is null)
            return BadRequest(new { message = "Invalid role ID" });

        if (!string.IsNullOrEmpty(request.Password) && request.Password.Length < 6)
            return BadRequest(new { message = "Password must be at least 6 characters" });

        try
        {
            userService.UpdateUser(id, request.Username, request.RoleId, request.Password);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteUser(int id)
    {
        if (userService.GetUser(id) is null)
            return NotFound(new { message = "User not found" });

        if (id == ActorId)
            return BadRequest(new { message = "You cannot delete your own account." });

        try
        {
            userService.DeleteUser(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}