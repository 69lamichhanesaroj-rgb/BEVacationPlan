using Infra;
using Service.Security;

namespace Service;

//Read it as a checklist: find the user, check the password, find the role, make the token.
//Any failure returns null.
//The return type LoginResponse? (with the question mark) means “a response or nothing”.
public class AuthService(AppDataConnection db, HashingPassword hp, TokenGen tg)
{
    public LoginResponse? Login(string email, string password)
    {
        email = email.Trim().ToLowerInvariant();
        var user = db.Users.SingleOrDefault(u => u.Email == email);
        if (user is null)
            return null;

        if (!hp.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            return null;

        var role = db.Roles.SingleOrDefault(r => r.Id == user.RoleId);
        if (role is null)
            return null;

        return new LoginResponse(tg.CreateToken(user, role.RoleName), user.Id, user.Email, role.RoleName);
    }
}