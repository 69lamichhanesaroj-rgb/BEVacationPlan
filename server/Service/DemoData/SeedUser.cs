using Infra;
using Infra.Entities;
using LinqToDB;
using Service.Security;

namespace Service.DemoData;

public class SeedUser(AppDataConnection db, HashingPassword hp)
{
    private void Create(string email, string roleName, string password)
    {
        if (db.Users.Any(u => u.Email == email))
        {
            return;
        }

        var role = db.Roles.SingleOrDefault(r => r.RoleName == roleName)
                   ?? throw new InvalidOperationException($"Role '{roleName}' must be seeded before users.");

        var hash = hp.HashAndSaltPassword(password);

        db.Insert(new User
        {
            Email = email,
            RoleId = role.Id,
            PasswordHash = hash.Hash,
            PasswordSalt = hash.Salt
        });
    }

    public void SeedUsers()
    {
        Create("owner@businessesbjerg.com", RoleNames.SystemOwner, "owner123");
        Create("accountant@businessesbjerg.com", RoleNames.Accountant, "accountant123");
        Create("employee@businessesbjerg.com", RoleNames.Employee, "employee123");
    }
}