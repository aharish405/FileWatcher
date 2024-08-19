namespace FileWatcherApp.Models.Box
{
    public class BoxDeleteViewModel
    {
        public int BoxId { get; set; }
        public string BoxName { get; set; }
        public DateTime ScheduleTime { get; set; }
        public string CalendarName { get; set; }
    }

}
