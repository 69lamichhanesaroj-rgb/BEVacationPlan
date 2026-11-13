using Infra;

namespace Service;

public class RoleService(AppDataConnection db)
{
    public List<RoleDto> GetRoles()
    {
        return db.Roles.Select(r => new RoleDto(r.Id, r.RoleName)).ToList();
    }

    public RoleDto? GetRole(int id)
    {
        return db.Roles.Where(r => r.Id == id).Select(r => new RoleDto(r.Id, r.RoleName)).SingleOrDefault(); //This is a LINQ query that selects a single role by its ID and projects it into a RoleDto. If no role is found, it returns null.
    } //Select(r => new RoleDto(...)) converts entities into DTOs inside the query, so only the needed columns are read from the database.
}