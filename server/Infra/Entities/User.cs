using LinqToDB.Mapping;
namespace Infra.Entities;

[Table(Name="Users")]
public class User
{
    [PrimaryKey, Identity]
    public int Id { get; set; }

    [Column(Name = "Username"), NotNull]
    public string Username { get; set; }= string.Empty;

    [Column(Name = "PasswordHash"), NotNull]
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    [Column(Name = "RoleId"), NotNull]
    public int RoleId { get; set; }
    
    [Column(Name = "PasswordSalt"), NotNull]
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
    

    [Association(ThisKey = nameof(RoleId), OtherKey = nameof(Role.Id), CanBeNull = false)]
    public Role Role { get; set; } = null!;
}

