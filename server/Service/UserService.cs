using Infra;
using Infra.Entities;
using LinqToDB;
using Service.Security;

namespace Service;

public class UserService(AppDataConnection db, HashingPassword hp)
{
    public List<UserDto> GetUsers()
    {
        return (from u in db.Users
                join r in db.Roles on u.RoleId equals r.Id
                select new UserDto(u.Id, u.Email, r.RoleName)).ToList();
    }

    public UserDto? GetUser(int id)
    {
        return (from u in db.Users
                join r in db.Roles on u.RoleId equals r.Id
                where u.Id == id
                select new UserDto(u.Id, u.Email, r.RoleName)).SingleOrDefault();
    }

    // NEW (was missing from Step 7): only ONE accountant and ONE system owner may exist.
    // exceptUserId = the user we are saving, so the current holder can keep their own role.
    private void EnsureRoleIsFree(int roleId, int exceptUserId)
    {
        var role = db.Roles.Single(r => r.Id == roleId);
        if (role.RoleName == RoleNames.Employee)
            return;

        if (db.Users.Any(u => u.RoleId == roleId && u.Id != exceptUserId))
            throw new InvalidOperationException($"There can only be one {role.RoleName}.");
    }

    // FIX: returns the new user's id (int instead of void). EmployeeService needs it.
    public int CreateUser(string email, int roleId, string password)
    {
        email = email.Trim().ToLowerInvariant();
        if (db.Users.Any(u => u.Email == email))
            throw new InvalidOperationException("Email already exists.");

        EnsureRoleIsFree(roleId, 0);   // 0 = nobody to ignore, ids start at 1

        var result = hp.HashAndSaltPassword(password);

        // InsertWithInt32Identity = insert the row AND give back the id the database assigned
        return db.InsertWithInt32Identity(new User
        {
            Email = email,
            RoleId = roleId,
            PasswordHash = result.Hash,
            PasswordSalt = result.Salt
        });
    }

    public void UpdateUser(int id, string email, int roleId, string? password)
    {
        var user = db.Users.SingleOrDefault(u => u.Id == id)
                   ?? throw new InvalidOperationException("User not found.");

        email = email.Trim().ToLowerInvariant();
        if (db.Users.Any(u => u.Email == email && u.Id != id))
            throw new InvalidOperationException("Email already exists.");

        EnsureRoleIsFree(roleId, id);

        user.Email = email;
        user.RoleId = roleId;

        if (!string.IsNullOrEmpty(password))
        {// hashing on create and change of password.
            var hash = hp.HashAndSaltPassword(password);
            user.PasswordHash = hash.Hash;
            user.PasswordSalt = hash.Salt;
        }

        db.Update(user);
    }

    public void DeleteUser(int id)
    {
        var user = db.Users.SingleOrDefault(u => u.Id == id)
                   ?? throw new InvalidOperationException("User not found.");

        if (db.Employees.Any(e => e.UserId == id))
        {
            throw new InvalidOperationException("Deactivate the employee instead of deleting the user, history must be kept.");
        }

        // NEW: the one accountant / system owner must never disappear.
        var role = db.Roles.Single(r => r.Id == user.RoleId);
        if (role.RoleName != RoleNames.Employee)
            throw new InvalidOperationException($"The {role.RoleName} account cannot be deleted. Change its email instead.");

        db.Delete(user);
    }
}