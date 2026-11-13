using LinqToDB.Mapping;
namespace Infra.Entities;

[Table("Roles")]
public class Role
{
    [PrimaryKey, Identity]
    public int Id { get; set; }

    [Column(Name = "RoleName"), NotNull]
    public string RoleName { get; set; }= string.Empty;

}