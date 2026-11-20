namespace Infra;

using LinqToDB;
using LinqToDB.Data;
using Infra.Entities;

public class AppDataConnection(DataOptions options) : DataConnection(options)
{
    public ITable<Role> Roles => this.GetTable<Role>();
    public ITable<User> Users => this.GetTable<User>();
    
    public ITable<WorkScheduleDay> WorkScheduleDays => this.GetTable<WorkScheduleDay>();
    
    public ITable<Employee> Employees => this.GetTable<Employee>();
}