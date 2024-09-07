using FileWatcherApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileWatcherApp.Data
{
    public class FileWatcherContext : DbContext
    {
        public FileWatcherContext(DbContextOptions<FileWatcherContext> options) : base(options) { }

        public DbSet<Box> Boxes { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobStatus> JobStatuses { get; set; }
        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<CalendarDay> CalendarDays { get; set; }
        public DbSet<ExcludeCalendar> ExcludeCalendars { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
    }
}
