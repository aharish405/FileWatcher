using FileWatcherApp.Data.Entities;

namespace FileWatcherApp.Models.Box
{
    public class BoxIndexViewModel
    {
        public int BoxId { get; set; }
        public string BoxName { get; set; }
        public DateTime ScheduleTime { get; set; }
        public string CalendarName { get; set; }
        public IEnumerable<CalendarDay> CalendarDays { get; set; }
        public ScheduleType ScheduleType { get; set; }
        public string Timezone { get; set; }
    }

}
