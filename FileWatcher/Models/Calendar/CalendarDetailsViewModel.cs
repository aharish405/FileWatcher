namespace FileWatcherApp.Models.Calendar
{
    public class CalendarDetailsViewModel
    {
        public int CalendarId { get; set; }
        public string Name { get; set; }
        public string ScheduleType { get; set; }
        public string Days { get; set; }
        public string Timezone { get; set; }
        public string Description { get; set; }
    }

}
