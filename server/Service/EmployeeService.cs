using System.Globalization;
using Infra;
using Infra.Entities;
using LinqToDB;
using Service.Security;

namespace Service;

public class EmployeeService(AppDataConnection db, UserService userService)
{
    public int CreateEmployee(CreateEmployeeRequest request)
    {
        if (request.HireDate == default)
            throw new ArgumentException("Hire date must be provided.");   // FIX: bad input = ArgumentException (controller -> 400)

        var schedule = ParseSchedule(request.Days); // validate BEFORE touching the database

        using var transaction = db.BeginTransaction(); // transaction means all or nothing, if anything fails, the database is rolled back to its previous state.(like a bank transfer)

        var employeeRole = db.Roles.Single(r => r.RoleName == RoleNames.Employee); // check the role and throws exception if not found, which will roll back the transaction and return 500 Internal Server Error to the client.

        var userId = userService.CreateUser(request.Email, employeeRole.Id, request.Password); //create the login with hashed password and returns a new  user id

        var employeeId = db.InsertWithInt32Identity(new Employee
        {
            UserId = userId,
            FullName = request.FullName.Trim(),
            HireDate = request.HireDate
        });

        // The first schedule starts on the Monday of the week of the hire date, or on the hire date itself if it is a Monday.
        var validFrom = MondayOnOrBefore(request.HireDate);
        foreach (var day in schedule)
        {
            db.Insert(new WorkScheduleDay
            {
                EmployeeId = employeeId,
                ValidFrom = validFrom,
                Weekday = day.Weekday,
                StartMinutes = day.Start,
                EndMinutes = day.End
            });
        }

        transaction.Commit();
        return employeeId;
    }

    public List<EmployeeDto> GetEmployees()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var employees = (from e in db.Employees
                         join u in db.Users on e.UserId equals u.Id
                         select new { e.Id, e.UserId, u.Email, e.FullName, e.HireDate, e.EndDate }).ToList();

        // Schedules are tiny (5 rows per version), so we load them and do the date logic in C#.
        var schedulesByEmployee = db.WorkScheduleDays.ToList().ToLookup(x => x.EmployeeId);

        // FIX 1: WeeklyHoursOn is a method of THIS class, not of the employee object: no "e." in front.
        // FIX 2: EmployeeDto has 7 values, e.EndDate was missing.
        return employees
            .Select(e => new EmployeeDto(e.Id, e.UserId, e.Email, e.FullName, e.HireDate, e.EndDate,
                WeeklyHoursOn(schedulesByEmployee[e.Id].ToList(), today)))
            .ToList();
    }

    public void DeactivateEmployee(int employeeId, DateOnly endDate)
    {
        var employee = db.Employees.SingleOrDefault(e => e.Id == employeeId)
                       ?? throw new KeyNotFoundException("Employee not found.");        // FIX: the controller catches KeyNotFoundException -> 404

        if (employee.EndDate is not null)      // works now that Employee.EndDate is DateOnly?
            throw new InvalidOperationException("Employee is already deactivated.");
        if (endDate < employee.HireDate)
            throw new ArgumentException("End date cannot be before hire date.");        // FIX: bad input -> 400

        employee.EndDate = endDate;
        db.Update(employee);
    }

    // Helper methods

    private static List<(int Weekday, int Start, int End)> ParseSchedule(List<WorkDayDto> days)   // FIX: DTO was misspelled WorkDAyDto
    {
        var result = new List<(int Weekday, int Start, int End)>();
        var seen = new HashSet<int>(); // to check for duplicates,HashSet is a collection that contains no duplicate elements, and it provides fast lookup for checking if an element is already present in the set.

        foreach (var day in days)
        {
            if (day.Weekday < 1 || day.Weekday > 5)
                throw new ArgumentException("Weekday must be 1-5.");

            if (!seen.Add(day.Weekday))
                throw new ArgumentException($"Weekday is listed twice: {day.Weekday}");

            if (day.From is null && day.To is null)
            {
                continue; // day off
            }

            if (day.From is null || day.To is null)
                throw new ArgumentException($"Both From and To must be provided for weekday {day.Weekday}.");

            var start = ToMinutes(day.From);
            var end = ToMinutes(day.To);

            if (start >= end)
                throw new ArgumentException($"Weekday {day.Weekday}: To must be later than From.");
            result.Add((day.Weekday, start, end));
        }
        if (result.Count == 0)
            throw new ArgumentException("At least one workday must be provided.");
        return result;
    }

    // Converts "HH:mm" to minutes since midnight, e.g. "09:30" => 570
    // TryParseExact is used to parse the time string in a specific format, and it returns a boolean indicating whether the parsing was successful or not.
    // If the parsing fails, an ArgumentException is thrown with a message indicating the expected format.
    private static int ToMinutes(string text)
    {   // FIX: the opening brace was missing
        if (!TimeOnly.TryParseExact(text, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
            throw new ArgumentException($"Invalid time format: {text}. Expected HH:mm.");
        return time.Hour * 60 + time.Minute;
    }

    private static DateOnly MondayOnOrBefore(DateOnly date)
    {
        // (n+6) % 7 means that Monday is 0, Tuesday is 1, ..., Sunday is 6. So if today is Wednesday (3), then daysSinceMonday = (3+6)%7 = 2, and we subtract 2 days to get back to Monday.
        var daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;
        return date.AddDays(-daysSinceMonday);   // FIX: the semicolon was missing. Subtract that many days to get back to Monday
    }

    private static decimal WeeklyHoursOn(List<WorkScheduleDay> rows, DateOnly date)
    {
        if (rows.Count == 0)
            return 0;

        // Find the most recent version of the schedule that is valid on or before the given date. If there is no such version, use the oldest version.
        var started = rows.Where(r => r.ValidFrom <= date).ToList();
        var version = started.Count > 0 ? started.Max(r => r.ValidFrom) : rows.Min(r => r.ValidFrom);

        // Sum the minutes for that version and convert to hours, rounded to 2 decimal places.
        var minutes = rows.Where(r => r.ValidFrom == version).Sum(r => r.EndMinutes - r.StartMinutes);
        return Math.Round(minutes / 60m, 2);
    }
}