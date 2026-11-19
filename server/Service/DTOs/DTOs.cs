using System.ComponentModel.DataAnnotations;

namespace Service;

//A DTO (data transfer object) is the exact shape of what goes in or out of the API.
//We use them so we never expose entities like User directly, since that would leak the hash and salt.

public record LoginRequest([Required, EmailAddress] string Email, [Required] string Password); //[Required] means ASP.NET automatically answers 400 Bad Request if someone sends a login with no username or password
public record LoginResponse(string Token, int Id, string Email, string Role);

public record RoleDto(int Id, string RoleName);

public record UserDto(int Id, string Email, string Role);

public record WorkDayDto(int Weekday, string? From, string? To);  // From/To as "HH:mm". Both null = day off.


public record CreateUserRequest(
    [Required, EmailAddress] string Email,
    int RoleId,
    [Required, MinLength(6)] string Password);

public record UpdateUserRequest(
    [Required, EmailAddress] string Email,
    int RoleId,
    string? Password); //Password is string? (nullable) because on an update, leaving it empty means “keep the current password”.
    
    
public record CreateEmployeeRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required, MinLength(2)] string FullName,
    DateOnly HireDate,
    [Required] List<WorkDayDto> Days);  


public record EmployeeDto(int Id,int UserId, string Email, string FullName, DateOnly HireDate,DateOnly? EndDate, decimal WeeklyHours);

public record DeactivateEmployeeRequest(DateOnly EndDate); // this is a soft delete pattern the record is kept for history