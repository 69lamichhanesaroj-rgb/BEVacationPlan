using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace API.Controllers;

//The controller contains no logic. It receives the request, calls the service, and chooses the HTTP status
[ApiController]
[Route("api/[controller]")] //[Route("api/[controller]")] takes the class name minus “Controller”, so this one lives at /api/auth, and the method adds /login.
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous] //login must be reachable without a token, since we don’t have one yet.
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        var result = authService.Login(request.Username, request.Password);

        return result is null
            ? Unauthorized(new { message = "Invalid username or password" }) //Unauthorized returns 401 Unauthorized, which is the correct response for a failed login. We don’t say “username not found” or “password wrong” because that would give away information to an attacker.
            : Ok(result);
    }
}