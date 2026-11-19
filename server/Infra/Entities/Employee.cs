using LinqToDB.Mapping;
namespace Infra.Entities;


[Table(Name = "Employees")]
public class Employee
{
    [PrimaryKey, Identity]
    public int Id { get; set; }
    
    [Column(Name = "UserId"),NotNull]
    public int UserId { get; set; }
    
    [Column(Name = "FullName"),NotNull]
    public string FullName { get; set; } = string.Empty;
    
    [Column(Name = "HireDate"),NotNull]
    public DateOnly HireDate { get; set; }
    
    [Column(Name = "EndDate")]
    public DateOnly? EndDate { get; set; }
    
}