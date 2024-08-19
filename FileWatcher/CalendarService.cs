using FileWatcherApp.Data;
using FileWatcherApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

public class CalendarService
{
    private readonly FileWatcherContext _context;

    public CalendarService(FileWatcherContext context)
    {
        _context = context;
    }

    public bool IsScheduled(DateTime date, int? calendarId)
    {
        if (!calendarId.HasValue)
        {
            return true; // No calendar means always scheduled
        }

        var calendar = _context.Calendars
            .Include(c => c.CalendarDays)
            .FirstOrDefault(c => c.Id == calendarId.Value);

        if (calendar == null)
        {
            return false; // Invalid calendar
        }

        return calendar.ScheduleType switch
        {
            ScheduleType.Daily => true,
            ScheduleType.Weekly => calendar.CalendarDays.Any(d => d.DayOfWeek == date.DayOfWeek),
            ScheduleType.Weekdays => date.DayOfWeek >= DayOfWeek.Monday && date.DayOfWeek <= DayOfWeek.Friday,
            ScheduleType.Custom => calendar.CalendarDays.Any(d => d.DayOfWeek == date.DayOfWeek),
            _ => false
        };
    }
}
