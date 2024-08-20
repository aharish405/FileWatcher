namespace FileWatcherApp.Data.Entities
{
    public class ExcludeCalendar
    {
        public int ExcludeCalendarId { get; set; }
        public string Name { get; set; }
        public List<DateTime> ExcludedDates { get; set; } // Consider using a separate entity if the list is large        
        public ICollection<Box> Box { get; set; }
    }
}
