using Infra;
using Infra.Entities;
using LinqToDB;
using Service.Security;

namespace Service.DemoData;

public class SeedUser(AppDataConnection db, HashingPassword hp)
{
    private void Create(string username, string roleName, string password)
    {
        if (db.Users.Any(u => u.Username == username))
        {
            return;
        }

        var role = db.Roles.SingleOrDefault(r => r.RoleName == roleName)
                   ?? throw new InvalidOperationException($"Role '{roleName}' must be seeded before users.");

        var hash = hp.HashAndSaltPassword(password);

        db.Insert(new User
        {
            Username = username,
            RoleId = role.Id,
            PasswordHash = hash.Hash,
            PasswordSalt = hash.Salt
        });
    }

    public void SeedUsers()
    {
        Create("accountant", RoleNames.Accountant, "accountant123");
        Create("employee", RoleNames.Employee, "employee123");
    }
}