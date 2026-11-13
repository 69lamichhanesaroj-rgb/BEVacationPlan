using System.ComponentModel.DataAnnotations;

namespace Service;

//A DTO (data transfer object) is the exact shape of what goes in or out of the API.
//We use them so we never expose entities like User directly, since that would leak the hash and salt.

public record LoginRequest([Required] string Username, [Required] string Password); //[Required] means ASP.NET automatically answers 400 Bad Request if someone sends a login with no username or password
public record LoginResponse(string Token, int Id, string Username, string Role);

public record RoleDto(int Id, string RoleName);

public record UserDto(int Id, string Username, string Role);

public record CreateUserRequest(
    [Required, MinLength(3)] string Username,
    int RoleId,
    [Required, MinLength(6)] string Password);

public record UpdateUserRequest(
    [Required, MinLength(3)] string Username,
    int RoleId,
    string? Password); //Password is string? (nullable) because on an update, leaving it empty means “keep the current password”.