namespace FileWatcherApp.Data.Entities
{
    public class CalendarDay
    {
        public int Id { get; set; }
        public int CalendarId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public Calendar Calendar { get; set; }
    }
}
