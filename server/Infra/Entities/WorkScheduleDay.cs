using LinqToDB.Mapping;
namespace Infra.Entities;

[Table(Name = "WorkScheduleDays")]
public class WorkScheduleDay
{
    [PrimaryKey, Identity]
    public int Id { get; set; }
    
    [Column(Name = "EmployeeId"), NotNull]
    public int EmployeeId { get; set; }
    
    [Column(Name = "ValidFrom"), NotNull]
    public DateOnly ValidFrom { get; set; }  // Always a Monday
    
    [Column(Name = "Weekday"), NotNull]
    public int Weekday { get; set; } // 1=Monday, 2=Tuesday, ...5=Friday
    
    [Column(Name = "StartMinutes"), NotNull]
    public int StartMinutes { get; set; }  // 08:00 = 480
    
    [Column(Name = "EndMinutes"), NotNull]
    public int EndMinutes { get; set; } // 15:30 = 930
    
}